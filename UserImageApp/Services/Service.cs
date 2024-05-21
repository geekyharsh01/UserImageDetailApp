// These namespaces are necessary for working with MongoDB and GridFS in a .NET application.
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using UserImageApp.Models;

namespace UserImageApp.Services
{
    // The Service class provides methods to interact with MongoDB and GridFS for storing and retrieving images and their associated data.
    public class Service
    {
        // Private fields to hold the database and GridFS bucket instances.
        private readonly IMongoDatabase _database;
        private readonly GridFSBucket _bucket;

        // Constructor that initializes the MongoDB client, database, and GridFS bucket.
        // It uses the configuration to get the MongoDB connection string.
        public Service(IConfiguration configuration)
        {
            // Create a MongoClient using the connection string from configuration.
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));

            // Get the database named "ImageUploadDb".
            _database = client.GetDatabase("ImageUploadDb");

            // Initialize a new GridFSBucket for the database to manage large file storage.
            _bucket = new GridFSBucket(_database);
        }

        // Method to upload an image to GridFS. It takes a file stream and the file name as parameters.
        // It returns the ObjectId of the uploaded image.
        public async Task<ObjectId> UploadImageAsync(Stream stream, string fileName)
        {
            // Create upload options with metadata containing the file name.
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument { { "filename", fileName } }
            };

            // Upload the stream to GridFS and return the ObjectId of the stored file.
            return await _bucket.UploadFromStreamAsync(fileName, stream, options);
        }

        // Method to retrieve an image from GridFS as a byte array. It takes the ObjectId of the image as a parameter.
        public async Task<byte[]> GetImageAsync(ObjectId id)
        {
            // Download the file from GridFS using its ObjectId and return it as a byte array.
            return await _bucket.DownloadAsBytesAsync(id);
        }

        // Method to get the MongoDB collection that stores Upload documents.
        public IMongoCollection<Upload> GetUploadsCollection()
        {
            // Return the "uploads" collection from the database.
            return _database.GetCollection<Upload>("uploads");
        }
    }
}
