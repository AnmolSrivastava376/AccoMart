using System.ComponentModel.DataAnnotations;

namespace Data.Models.DTO
{
    public class CreateDeliveryServiceDto
    {
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public int DeliveryDays { get; set; }
    }
}
