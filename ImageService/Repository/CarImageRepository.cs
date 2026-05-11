namespace ImageService.Repository
{
    public class CarImageRepository : ICarImageRepository
    {
        private readonly CarImageDbContext _context;
        private readonly IMapper _mapper;
        public CarImageRepository(CarImageDbContext carImageDbContext, IMapper mapper)
        {
            _context = carImageDbContext;
            _mapper = mapper;

        }
        public async Task<(int, string)> CreateImage(NewCarImage newCarImage)
        {
            try
            {
                var carsImage = _mapper.Map<CarsImage>(newCarImage);
                await _context.AddAsync(carsImage);
                await _context.SaveChangesAsync();
                return (1, "Success");
            }
            catch (Exception ex)
            {
                return (0, "null");

            }



        }

        public async Task<bool> DeleteImage(int id)
        {
            var ExistingId = await _context.CarsImages.FindAsync(id);
            try
            {
                if (ExistingId == null)
                {
                    return false;
                }
                else
                {
                    _context.CarsImages.Remove(ExistingId);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> GetImageById(int id)
        {
            var ExistingId = await _context.CarsImages.FindAsync(id);
            try
            {
                if (ExistingId == null)
                {
                    return "Image not exists!";
                }
                else
                {
                    return ExistingId.ImageUrl;
                }
            }
            catch (Exception ex)
            {
                return $"Error while fetching {ex.Message}";

            }

        }

        public async Task<(int, string)> UpdateImage(int id, UpdateCarImage updateCarImage)
        {
            var ExistingId = await _context.CarsImages.FindAsync(id);

            try
            {
                if (ExistingId == null)
                {
                    return (0, "Cannot find the id");
                }
                else
                {
                    _mapper.Map(updateCarImage, ExistingId);
                    await _context.SaveChangesAsync();
                    return (1, "Successfully updated");
                }
            }
            catch (Exception ex)
            {
                return (-1, $"Error {ex.Message}");
            }

        }

        public async Task<IEnumerable<UpdateCarImage>> GetImageByCarID(int id)
        {

            try
            {
                var imageUrls = await _context.CarsImages
                .Where(ci => ci.CarId == id)
                .Select(ci => new UpdateCarImage
                {
                    ImageUrl = ci.ImageUrl
                })
                .ToListAsync();

                return imageUrls;
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return new List<UpdateCarImage>();
            }

        }

        public async Task<UpdateCarImage?> GetFrontImageUrlByCarIdAsync(int carId)
        {
            var image = await _context.CarsImages
                .Where(img => img.CarId == carId &&
                             (img.ImageName.ToLower() == "frontview" || img.ImageName.ToLower() == "cover"))
                .FirstOrDefaultAsync();

            if (image == null) return null;

            return new UpdateCarImage { ImageUrl = image.ImageUrl };
        }

    }
}
