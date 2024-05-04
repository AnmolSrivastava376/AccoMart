using Data.Models.DTO;
using Data.Models;

namespace Service.Services.Interface
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetAllProductsAsync(int id, string orderBy);
        Task<List<Category>> GetAllCategoriesAsync(string prefix);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(string categoryName);
        Task<Product> CreateProductAsync(ProductDto productDto);
        Task<Category> UpdateCategoryAsync(string categoryName, string NewCategoryName);
        Task<Product> UpdateProductAsync(int productId, UpdateProductDto productDto);
        Task DeleteCategoryAsync(int CategoryId);
        Task DeleteProductAsync(int ProductId);
        Task<Product> GetProductBySearchNameAsync(string prefix);
        //Task<Product> GetProductByPriceOrderAsync(string OrderBy);
    }
}
