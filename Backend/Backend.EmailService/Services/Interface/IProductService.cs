using Data.Models.DTO;
using Data.Models;

namespace Service.Services.Interface
{
    public interface IProductService
    {
        Task<Data.Models.Product> GetProductByIdAsync(int id);
        Task<List<Data.Models.Product>> GetAllProductsByCategoryAsync(int id, string orderBy);
        Task<List<Data.Models.Product>> GetAllProductsAsync();
        Task<List<Data.Models.Product>> GetProductsByPageNoAsync(int id, int pageNo, int pageSize);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> GetCategoryByNameAsync(string name);
        Task<Category> CreateCategoryAsync(string categoryName);
        Task<Data.Models.Product> CreateProductAsync(Data.Models.DTO.Product productDto);
        Task<Category> UpdateCategoryAsync(int Id, string NewCategoryName);
        Task<Data.Models.Product> UpdateProductAsync(int productId, UpdateProduct productDto);
        Task DeleteCategoryAsync(int CategoryId);
        Task DeleteProductAsync(int ProductId);
        Task<List<Data.Models.Product>> GetProductBySearchNameAsync(string prefix);
        Task<List<Data.Models.Product>> GetProductsByCategoryNameAsync(string prefix);

    }
}
