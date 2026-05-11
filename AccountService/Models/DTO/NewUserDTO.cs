using System.ComponentModel.DataAnnotations;

namespace AccountService.Models.DTO
{
    public class NewUserDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public string Address { get; set; } = "Not Provided";
    }
}
