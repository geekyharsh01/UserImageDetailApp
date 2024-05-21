// The following namespaces are required to use MongoDB-specific classes and attributes.
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserImageApp.Models
{
    // The Upload class represents a single upload record in the MongoDB database.
    public class Upload
    {
        // The Id property is annotated with [BsonId] to indicate that this field will be used
        // as the unique identifier for the document in the MongoDB collection.
        // ObjectId is a MongoDB-specific type that represents a unique identifier for documents.
        [BsonId]
        public ObjectId Id { get; set; }

        // The Description property will store any descriptive text provided by the user 
        // when they upload an image. This is a regular string field.
        public string Description { get; set; }

        // The ImageId property will store the unique identifier of the image file
        // that has been uploaded to GridFS. This field is also of type ObjectId,
        // which allows for easy reference and retrieval from the GridFS bucket.
        public ObjectId ImageId { get; set; }
    }
}
