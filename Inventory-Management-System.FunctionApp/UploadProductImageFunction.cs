using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Inventory_Management_System.Infrastructure.Interfaces;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;


namespace Inventory_Management_System.FunctionApp
{
    public class UploadProductImageFunction
    {
        private readonly ILogger<UploadProductImageFunction> _logger;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IApplicationDbContext _context;

        public UploadProductImageFunction(ILogger<UploadProductImageFunction> logger, BlobServiceClient blobServiceClient, IApplicationDbContext context)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
            _context = context;
        }

        [Function("UploadProductImageFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "products/{productId}/upload-image")] HttpRequest req, int productId)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ImageUploadRequest>(requestBody);

                if (string.IsNullOrEmpty(data.ImageURL))
                    return new BadRequestObjectResult("Image URL is required.");

                // Download the image from ImageURL
                using HttpClient httpClient = new();
                using var imageStream = await httpClient.GetStreamAsync(data.ImageURL);

                string fileName = $"{Guid.NewGuid()}-{data.ImageName}";
                string containerName = "product-images";

                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(imageStream, new BlobHttpHeaders { ContentType = data.ImageType });

                string blobUrl = blobClient.Uri.ToString();

                // Update product in DB
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return new NotFoundObjectResult("Product not found.");

                product.ImageUrl = blobUrl;
                product.ImageName = data.ImageName;
                product.ImageType = data.ImageType;
                await _context.SaveChangesAsync(new CancellationToken());

                return new OkObjectResult(new { Message = "Image uploaded successfully!", ImageUrl = blobUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading image: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }
    }

    public class ImageUploadRequest
    {
        public string ImageName { get; set; }
        public string ImageType { get; set; }
        public string ImageURL { get; set; }
    }
}
