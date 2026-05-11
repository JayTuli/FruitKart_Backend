using AccountService.Models;
using AccountService.Models.DTO;

namespace AccountService.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> UpdateUserAddressAsync(int id, UpdateUserDTO updateDTO);
    }
}
