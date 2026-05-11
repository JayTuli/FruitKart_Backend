using MenuServiceAPI.Models;
using MenuServiceAPI.Models.DTO;
using MenuServiceAPI.Repository


namespace MenuServiceAPI.Repository
{
    public class MenuRepository : IMenuRepository
    {
        private readonly MenuDbContext _db;
        private readonly IMapper _mapper;

        public MenuRepository(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<MenuItemDTO>> GetAllAsync()
        {
            var items = await _db.MenuItems.AsNoTracking().ToListAsync();
            return _mapper.Map<List<MenuItemDTO>>(items);
        }

        public async Task<MenuItemDTO?> GetByIdAsync(int id)
        {
            var item = await _db.MenuItems.AsNoTracking()
                                          .FirstOrDefaultAsync(m => m.Id == id);
            return item is null ? null : _mapper.Map<MenuItemDTO>(item);
        }

        public async Task<MenuItemDTO> CreateAsync(MenuItem menuItem)
        {
            await _db.MenuItems.AddAsync(menuItem);
            await _db.SaveChangesAsync();
            return _mapper.Map<MenuItemDTO>(menuItem);
        }

        public async Task<MenuItemDTO?> UpdateAsync(int id, MenuItem updated)
        {
            var existing = await _db.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
            if (existing is null) return null;

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.Category = updated.Category;
            existing.SpecialTag = updated.SpecialTag;
            existing.Price = updated.Price;

            if (!string.IsNullOrWhiteSpace(updated.ImageUrl))
            {
                existing.ImageUrl = updated.ImageUrl;
                existing.ImagePublicId = updated.ImagePublicId;
            }

            await _db.SaveChangesAsync();
            return _mapper.Map<MenuItemDTO>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _db.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
            if (item is null) return false;

            _db.MenuItems.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
