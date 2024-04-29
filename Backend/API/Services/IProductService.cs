using API.Models.DTO;
using API.Models;

namespace API.Services
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(string categoryName);
        Task<Product> CreateProductAsync(ProductDto productDto);
        Task<Category> UpdateCategoryAsync(string categoryName, string NewCategoryName);
        Task<Product> UpdateProductAsync(int productId, UpdateProductDto productDto);
        Task DeleteCategoryAsync(int CategoryId);
        Task DeleteProductAsync(int ProductId);    
    }
}
