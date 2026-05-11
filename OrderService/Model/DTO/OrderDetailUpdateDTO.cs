using System.ComponentModel.DataAnnotations;

namespace OrderService.Model.DTO
{
    public class OrderDetailUpdateDTO
    {
        [Required]
        public int OrderDetailId { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
