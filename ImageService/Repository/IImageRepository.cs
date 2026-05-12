using ImageService.Models.DTO;

namespace ImageService.Repository
{
    public interface IImageRepository
    {
        Task<UpdatedImagesDTO?> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string blobName);
        Task<List<UpdatedImagesDTO>> GetAllAsync();
    }
}