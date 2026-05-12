namespace ImageService.Repository
{
    public interface IAzureBlobService
    {
        Task<(string imageUrl, string blobName)> UploadAsync(IFormFile file);
        Task DeleteAsync(string blobName);
    }
}