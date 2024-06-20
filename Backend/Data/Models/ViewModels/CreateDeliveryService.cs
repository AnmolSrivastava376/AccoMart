using System.ComponentModel.DataAnnotations;

namespace Data.Models.ViewModels
{
    public class CreateDeliveryService
    {
        [Required]
        public string ImageUrl { get; set; }
        [Required(ErrorMessage ="Service Name is required")]
        public string ServiceName { get; set; }
        [Required(ErrorMessage ="Delivery Service Price is required")]
        public float Price { get; set; }
        [Required(ErrorMessage = "Delivery Days should be greater than 0"), Range(1, Int32.MaxValue)]
        public int DeliveryDays { get; set; }
    }
}
