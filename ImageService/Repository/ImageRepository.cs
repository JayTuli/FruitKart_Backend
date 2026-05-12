using AutoMapper;
using ImageService.Data;
using ImageService.Models;
using ImageService.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace ImageService.Repository
{
    public class ImageRepository : IImageRepository
    {
        private readonly FruitImageDbContext _db;
        private readonly IAzureBlobService _blobService;
        private readonly IMapper _mapper;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(
            FruitImageDbContext db,
            IAzureBlobService blobService,
            IMapper mapper,
            ILogger<ImageRepository> logger)
        {
            _db = db;
            _blobService = blobService;
            _mapper = mapper;
            _logger = logger;
        }

        // ── UPLOAD ────────────────────────────────────────────────────────────
        public async Task<UpdatedImagesDTO?> UploadImageAsync(IFormFile file)
        {
            try
            {
                // Azure handles the upload
                var (imageUrl, blobName) = await _blobService.UploadAsync(file);

                // Save record to SQL Server
                var image = new Images
                {
                    ImageName = file.FileName,
                    ImageUrl = imageUrl,
                    BlobName = blobName,
                    UploadedAt = DateTime.UtcNow
                };

                _db.Images.Add(image);
                await _db.SaveChangesAsync();

                return new UpdatedImagesDTO
                {
                    ImageUrl = imageUrl,
                    BlobName = blobName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload failed.");
                return null;
            }
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        public async Task<bool> DeleteImageAsync(string blobName)
        {
            try
            {
                // Delete from Azure
                await _blobService.DeleteAsync(blobName);

                // Delete from SQL Server
                var image = await _db.Images.FirstOrDefaultAsync(i => i.BlobName == blobName);
                if (image is not null)
                {
                    _db.Images.Remove(image);
                    await _db.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete failed. BlobName: {BlobName}", blobName);
                return false;
            }
        }

        // ── GET ALL ───────────────────────────────────────────────────────────
        public async Task<List<UpdatedImagesDTO>> GetAllAsync()
        {
            var images = await _db.Images.AsNoTracking().ToListAsync();
            return _mapper.Map<List<UpdatedImagesDTO>>(images);
        }
    }
}