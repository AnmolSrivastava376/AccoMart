using Data.Models.Product_Category.Product;
using System.ComponentModel.DataAnnotations;
namespace Data.Models.ViewModels
{
    public class OrderItem
    {
        [Required]
        public Product Product { get; set; }
        [Required(ErrorMessage ="Quantity should be provided"), Range(1,Int32.MaxValue)]
        public int Quantity { get; set; }
    }
}
