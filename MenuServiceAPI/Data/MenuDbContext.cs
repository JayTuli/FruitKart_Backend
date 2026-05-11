using MenuServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuServiceAPI.Data
{
    public class MenuDbContext : DbContext

    {
        public MenuDbContext(DbContextOptions<MenuDbContext> options) : base(options) { }

        public DbSet<MenuItem> MenuItems { get; set; }
    }
}