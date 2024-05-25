using Data.Models;
using Data.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Data.Repository.Interfaces
{
    public interface IProductRepository
    {

        Task<Product> GetProductById(int id);
        Task<Category> GetCategoryByName(string name);
        Task<List<Product>> GetAllProductsByCategoryAsync(int id,string orderBy);
        Task<List<Product>> GetProductsByPageNoAsync(int id, int pageNo, int pageSize);
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int id);
        Task<Category> CreateCategory(string categoryName);
        Task<Product> CreateProduct(ProductDto productDto);
        Task<Category> UpdateCategory(int Id, string NewCategoryName);
        Task<Product> UpdateProduct(int productId, UpdateProductDto productDto);
        Task DeleteCategory(int CategoryId);
        Task DeleteProduct(int productId);
        Task<List<Product>> GetProductBySearchName(string prefix);
        Task<List<Product>> GetProductsByCategoryName(string name);

        

    }
}
