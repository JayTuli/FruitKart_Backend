using AccountService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Data
{
    public class AccountDbContext : IdentityDbContext<ApplicationUser>  
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options): base(options) { }
        public DbSet<User> User { get; set; }
    }
}
