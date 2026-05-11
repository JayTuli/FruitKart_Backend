using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageService.Models
{
    public class Images
    {
        [Key]
        public int ImageId { get; set; }
        public string ImageName { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public int FruitId { get; set; }
    }
}
