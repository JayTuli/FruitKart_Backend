using AccountService.Models.DTO;
using AccountService.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AccountService.Models;

namespace AccountService.Controllers
{
        [ApiController]
        [Route("api/[controller]")]

    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }
        [HttpPost]
        [Route("register")]
        //public async Task<IActionResult> Register(NewUserDTO user)
        //{

        //    await _accountRepository.RegisterAsync(user, UserRoles.User);
        //    return Ok(new { message = "User registered successfully" });
        //}
        [HttpPost]
        public async Task<IActionResult> Register(NewUserDTO user)
        {
            try
            {
                var (status, message) = await _accountRepository.RegisterAsync(user, UserRoles.User);

                if (status == 1)
                {
                    return Ok(new { message });
                }
                else
                {
                    return BadRequest(new { error = message });
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error occurred during registration");

                return StatusCode(500, new { error = "An unexpected error occurred. Please try again later." });
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var response = await _accountRepository.LoginAsync(login);
            if (response.UserId == 0)
            {
                return Unauthorized(new { message = response.Token });
            }
            return Ok(new { UserId = response.UserId, Token = response.Token });
        }





    }
}