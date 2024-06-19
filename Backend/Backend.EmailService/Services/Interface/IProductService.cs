
using Data.Models.Product_Category.Category;
using Data.Models.Product_Category.Product;
using Data.Models.ViewModels;
using Data.Models.ViewModels.UpdateProduct;

namespace Service.Services.Interface
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetAllProductsByCategoryAsync(int id, string orderBy);
        Task<List<Product>> GetAllProductsAsync();

        Task<List<Product>> GetAllProductsPagewiseAsync(int pageNo, int pageSize,string userId);
        Task<List<Product>> GetProductsByPageNoAsync(int id, int pageNo, int pageSize);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> GetCategoryByNameAsync(string name);
        Task<Category> CreateCategoryAsync(string categoryName);
        Task<Product> CreateProductAsync(ViewProduct productDto);
        Task<Category> UpdateCategoryAsync(int Id, string NewCategoryName);
        Task<Product> UpdateProductAsync(int productId, UpdateProduct productDto);
        Task DeleteCategoryAsync(int CategoryId);
        Task DeleteProductAsync(int ProductId);
        Task<List<Product>> GetProductBySearchNameAsync(string prefix);
        Task<List<Product>> GetProductsByCategoryNameAsync(string prefix);

    }
}
