using ImageService.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageService.Data
{
    // random 
    public class FruitImageDbContext : DbContext
    {
        public FruitImageDbContext(DbContextOptions<FruitImageDbContext>options):base(options) { }
        public DbSet<Images> Images { get; set; }
    }
}
