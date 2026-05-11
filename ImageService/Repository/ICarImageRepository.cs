namespace ImageService.Repository
{
    public interface ICarImageRepository
    {
        public Task<(int, string)> CreateImage(NewCarImage newCarImage);
        public Task<(int, string)> UpdateImage(int id, UpdateCarImage updateCarImage);
        public Task<bool> DeleteImage(int id);

        public Task<string> GetImageById(int id);
        public Task<IEnumerable<UpdateCarImage>> GetImageByCarID(int id);
        Task<UpdateCarImage?> GetFrontImageUrlByCarIdAsync(int carId);

    }
}
