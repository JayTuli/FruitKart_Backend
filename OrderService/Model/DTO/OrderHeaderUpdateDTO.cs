using System.ComponentModel.DataAnnotations;

namespace OrderService.Model.DTO
{
    public class OrderHeaderUpdateDTO
    {
        [Required]
        public int OrderHeaderId { get; set; }
        public string PickUpName { get; set; } = string.Empty;
        public string PickUpPhoneNumber { get; set; } = string.Empty;
        public string PickUpEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
