namespace API.Models
{
    public class CartItem
    {
        public Product Product { get; set; }    
        /*public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; }
        public string CategoryName { get; set; }*/
        public int Quantity { get; set; }
    }
}
