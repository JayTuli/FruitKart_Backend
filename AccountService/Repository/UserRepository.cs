using AccountService.Models;
using AccountService.Data;
using AccountService.Models.DTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AccountDbContext _userDbContext;
        private readonly IMapper _mapper;

        public UserRepository(AccountDbContext context, IMapper mapper)
        {
            _userDbContext = context;
            _mapper = mapper;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userDbContext.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userDbContext.Users.ToListAsync();
        }


        public async Task<bool> UpdateUserAddressAsync(int UserId, UpdateUserDTO updateDTO)
        {
            var user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.UserId == UserId);
            if (user == null)
            {
                return false;
            }

            _mapper.Map(updateDTO, user);
            await _userDbContext.SaveChangesAsync();
            return true;
        }
    }
}