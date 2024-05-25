using Data.Models.DTO;
using Data.Models;

namespace Service.Services.Interface
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetAllProductsByCategoryAsync(int id, string orderBy);
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetProductsByPageNoAsync(int id, int pageNo, int pageSize);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(string categoryName);
        Task<Product> CreateProductAsync(ProductDto productDto);
        Task<Category> UpdateCategoryAsync(int Id, string NewCategoryName);
        Task<Product> UpdateProductAsync(int productId, UpdateProductDto productDto);
        Task DeleteCategoryAsync(int CategoryId);
        Task DeleteProductAsync(int ProductId);
        Task<List<Product>> GetProductBySearchNameAsync(string prefix);

        Task<List<Product>> GetProductsByCategoryNameAsync(string prefix);



    }
}
