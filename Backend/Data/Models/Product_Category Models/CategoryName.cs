using System.ComponentModel.DataAnnotations;

namespace Data.Models.Product_Category
{
    public class CategoryName
    {
        [Required(ErrorMessage ="Category Name is required")]
        public string name { get; set; }
    }
}
