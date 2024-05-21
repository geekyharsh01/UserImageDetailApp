// Necessary namespaces for the controller.
using Microsoft.AspNetCore.Mvc;
using UserImageApp.Models;
using UserImageApp.Services;
using System.Threading.Tasks;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;

namespace UserImageApp.Controllers
{
    // The HomeController handles the web requests for the image upload application.
    public class HomeController : Controller
    {
        // Private field to hold the reference to the MongoDB service.
        private readonly Service _mongoDbService;

        // Constructor that initializes the MongoDB service via dependency injection.
        public HomeController(Service mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        // GET: Home/Index
        // Returns the view for the main upload page.
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: Home/Index
        // Handles the form submission for uploading an image and adding a description.
        [HttpPost]
        public async Task<IActionResult> Index(string description, IFormFile image)
        {
            // Check if the image file is provided and has content.
            if (image != null && image.Length > 0)
            {
                // Upload the image to GridFS and get the image ID.
                var imageId = await _mongoDbService.UploadImageAsync(image.OpenReadStream(), image.FileName);

                // Create a new Upload object with the provided description and image ID.
                var upload = new Upload
                {
                    Description = description,
                    ImageId = imageId
                };

                // Get the uploads collection and insert the new Upload object.
                var collection = _mongoDbService.GetUploadsCollection();
                await collection.InsertOneAsync(upload);

                // Redirect to the List action to view all uploads.
                return RedirectToAction("List");
            }

            // If no image is provided, return the same view.
            return View();
        }

        // GET: Home/List
        // Retrieves and displays a list of all uploaded images and their descriptions.
        [HttpGet]
        public async Task<IActionResult> List()
        {
            // Get the uploads collection.
            var collection = _mongoDbService.GetUploadsCollection();

            // Retrieve all documents from the uploads collection.
            var uploads = await collection.Find(_ => true).ToListAsync();

            // Return the view with the list of uploads.
            return View(uploads);
        }

        // GET: Home/Edit/{id}
        // Displays the edit form for a specific upload identified by its ID.
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            // Get the uploads collection.
            var collection = _mongoDbService.GetUploadsCollection();

            // Find the upload document by its ID.
            var upload = await collection.Find(x => x.Id == new ObjectId(id)).FirstOrDefaultAsync();

            // Return the view with the upload data to be edited.
            return View(upload);
        }

        // POST: Home/Edit/{id}
        // Handles the form submission for editing an upload's description.
        [HttpPost]
        public async Task<IActionResult> Edit(string id, string description)
        {
            // Get the uploads collection.
            var collection = _mongoDbService.GetUploadsCollection();

            // Create a filter to find the document by its ID.
            var filter = Builders<Upload>.Filter.Eq(x => x.Id, new ObjectId(id));

            // Create an update definition to set the new description.
            var update = Builders<Upload>.Update.Set(x => x.Description, description);

            // Update the document in the collection.
            await collection.UpdateOneAsync(filter, update);

            // Redirect to the List action to view all uploads.
            return RedirectToAction("List");
        }

        // GET: Home/ViewImage/{id}
        // Retrieves and displays an image from GridFS.
        [HttpGet]
        public async Task<IActionResult> ViewImage(string id)
        {
            // Get the image bytes from GridFS using the image ID.
            var image = await _mongoDbService.GetImageAsync(new ObjectId(id));

            // Return the image file with the appropriate MIME type.
            return File(image, "image/jpeg");
        }
    }
}
