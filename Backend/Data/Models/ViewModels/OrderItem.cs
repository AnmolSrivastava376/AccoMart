using Data.Models.Product_Category.Product;
namespace Data.Models.ViewModels
{
    public class OrderItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
