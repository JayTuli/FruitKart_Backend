using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AccountService.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}
