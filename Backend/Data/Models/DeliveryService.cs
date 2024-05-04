using System.ComponentModel.DataAnnotations;
namespace Data.Models { 
    public class DeliveryService
    {
        public int DServiceId { get; set; }
        public string ImageUrl { get; set; }
        public string ServiceName { get; set; } 
        public float Price { get; set; }
        public int DeliveryDays { get; set; }
    }
}
