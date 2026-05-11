using AccountService.Models.DTO;

namespace AccountService.Repository
{
    public interface IAccountRepository
    {
        Task<(int, string)> RegisterAsync(NewUserDTO user, string role);

        Task<LoginResponseDTO> LoginAsync(LoginDTO login);
    }
}
