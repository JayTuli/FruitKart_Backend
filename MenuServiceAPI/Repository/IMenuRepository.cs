using MenuServiceAPI.Models;
using MenuServiceAPI.Models.DTO;

namespace MenuServiceAPI.Repository
{
    public interface IMenuRepository
    {
        Task<List<MenuItemDTO>> GetAllAsync();
        Task<MenuItemDTO?> GetByIdAsync(int id);
        Task<MenuItemDTO> CreateAsync(MenuItem menuItem);
        Task<MenuItemDTO?> UpdateAsync(int id, MenuItem menuItem);
        Task<bool> DeleteAsync(int id);
    }
}
