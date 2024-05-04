using System.ComponentModel.DataAnnotations;

namespace API.Models.DTO
{
    public class CreateDeliveryServiceDto
    {
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public float price { get; set; }
        [Required]
        public int DeliveryDays { get; set; }
    }
}
