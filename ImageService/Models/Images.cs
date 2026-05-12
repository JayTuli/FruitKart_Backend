using System.ComponentModel.DataAnnotations;

namespace ImageService.Models
{
    public class Images
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ImageName { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        //[Required]
        //public int FruitId { get; set; }
        [Required]
        public string BlobName { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}