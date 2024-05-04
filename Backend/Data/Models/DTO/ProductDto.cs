namespace Data.Models.DTO
{
    public class ProductDto
    {
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; }
       // public int CategoryId { get; set; }
        public string CategoryName { get; set; }
       
    }
}
