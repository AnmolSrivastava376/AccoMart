using System.ComponentModel.DataAnnotations;
namespace Data.Models.Delivery
{
    public class DeliveryService
    {
        [Required]
        public int DServiceId { get; set; }

        [Required,Url]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage ="Service Name is required")]
        public string ServiceName { get; set; }

        [Required,Range(0.01, double.MaxValue, ErrorMessage = "Price should be greater than zero.")]
        public float Price { get; set; }

        [Required,Range(1,Int32.MaxValue,ErrorMessage ="Delivery days should be greater than zero")]
        public int DeliveryDays { get; set; }
    }
}
