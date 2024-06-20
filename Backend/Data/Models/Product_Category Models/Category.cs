using System.ComponentModel.DataAnnotations;

namespace Data.Models.Product_Category.Category
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; }

    }
}
