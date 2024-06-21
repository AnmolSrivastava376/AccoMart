using System.ComponentModel.DataAnnotations;

namespace Data.Models.OrderModels
{
    public class OrderHistory
    {
        [Required]
        public int ProductId { get; set; }
        [Required, Range(1, Int32.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}
