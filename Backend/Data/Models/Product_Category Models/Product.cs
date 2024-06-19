using System.ComponentModel.DataAnnotations;

namespace Data.Models.Product_Category.Product
{
    public class Product
    {
        
        public int ProductId { get; set; }
        [Required(ErrorMessage ="Product Name is required")]
        public string ProductName { get; set; }
        [Required(ErrorMessage ="Product Description is required")]
        public string ProductDesc { get; set; }
        [Required(ErrorMessage ="Product price is required"),Range(0.1,double.MaxValue)]
        public decimal ProductPrice { get; set; }
        [Required(ErrorMessage ="Product image url is required"),Url]
        public string ProductImageUrl { get; set; }
        [Required(ErrorMessage ="CategoryID is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage ="Stock should be provided")]
        public int Stock { get; set; }

    }

}
