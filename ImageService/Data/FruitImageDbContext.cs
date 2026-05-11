using ImageService.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageService.Data
{
    // random name for the database context class, it can be anything but it should be unique and meaningful
    public class FruitImageDbContext : DbContext
    {
        public FruitImageDbContext(DbContextOptions<FruitImageDbContext>options):base(options) { }
        public DbSet<Images> Images { get; set; }
    }
}
