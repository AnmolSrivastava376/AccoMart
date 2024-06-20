using System.ComponentModel.DataAnnotations;

namespace Data.Models.ViewModels
{
    public class ProductOrder
    {
        [Required(ErrorMessage ="Error message is requied")]
        public string UserId { get; set; }

        [Required(ErrorMessage ="Address Id is requied")]
        public int AddressId { get; set; }

        [Required(ErrorMessage ="Delivery Id is requied")]
        public int DeliveryId{ get; set; }

        [Required(ErrorMessage = "Product Id is requied")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required"), Range(0, Int32.MaxValue)]
        public int quantity   {  get; set; }

    }
}
