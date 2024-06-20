using System.ComponentModel.DataAnnotations;

namespace Data.Models.OrderModels
{ 
    public class Orders
    {

        [Required(ErrorMessage = "Order Id is required")]
        public int OrderId { get; set; }
        [Required(ErrorMessage ="Order Date is required")]
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "UserID is required")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "AddressId is required")]
        public int AddressId { get; set; }
        [Required(ErrorMessage = "OrderAmount is required")]
        public float OrderAmount {  get; set; }
        [Required(ErrorMessage = "OrderTime is required")]
        public TimeOnly OrderTime { get; set; }
        public int? ProductId { get; set; }
        public int? CartId { get; set; }
        [Required(ErrorMessage = "Delviery Service not provided")]
        public int DeliveryServiceID { get; set; }
        public Boolean isCancelled { get; set; } = true;
    }
}
