using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace ImageService.Repository
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AzureBlobService> _logger;
        private const string ContainerName = "fruitkartcontainer";

        public AzureBlobService(IConfiguration config, ILogger<AzureBlobService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<(string imageUrl, string blobName)> UploadAsync(IFormFile file)
        {
            var connectionString = _config["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException("AzureStorage:ConnectionString not configured.");

            var containerClient = new BlobContainerClient(connectionString, ContainerName);

            // Unique blob name prevents collisions
            var blobName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var blobClient = containerClient.GetBlobClient(blobName);

            // Upload with correct content type
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

            // SAS URL valid 1 year — container stays Private
            var sasUri = blobClient.GenerateSasUri(
                BlobSasPermissions.Read,
                DateTimeOffset.UtcNow.AddYears(1));

            return (sasUri.ToString(), blobName);
        }

        public async Task DeleteAsync(string blobName)
        {
            var connectionString = _config["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException("AzureStorage:ConnectionString not configured.");

            var containerClient = new BlobContainerClient(connectionString, ContainerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}