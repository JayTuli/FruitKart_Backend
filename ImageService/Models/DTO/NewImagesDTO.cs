//using System.ComponentModel.DataAnnotations;
//namespace ImageService.Models.DTO
//{
//    public class NewImagesDTO
//    {
//        public string ImageName { get; set; }
//        [Required]
//        public string ImageUrl { get; set; }
//        [Required]
//        public int FruitId { get; set; }

//    }
//}
using System.ComponentModel.DataAnnotations;

namespace ImageService.Models.DTO
{
    public class NewImagesDTO
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}