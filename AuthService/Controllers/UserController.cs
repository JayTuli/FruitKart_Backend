using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AccountService.Repository;
using AccountService.Models.DTO;

namespace AccountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }


        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(user);
        }

        [HttpPut("UpdateAddress/{id}")]
        public async Task<IActionResult> UpdateUserAddress(int id, [FromBody] UpdateUserDTO updateDTO)
        {
            if (updateDTO == null || string.IsNullOrWhiteSpace(updateDTO.Address))
            {
                return BadRequest("Invalid address data.");
            }

            var isUpdated = await _userRepository.UpdateUserAddressAsync(id, updateDTO);

            if (!isUpdated)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok($"User ID {id}'s address has been updated successfully.");
        }



    }
}
