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
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int id);
        Task<Category> CreateCategory(string categoryName);
        Task<Product> CreateProduct(ProductDto productDto);
        Task<Category> UpdateCategory(string categoryName, string NewCategoryName);
        Task<Product> UpdateProduct(int productId, UpdateProductDto productDto);
        Task DeleteCategory(int CategoryId);
        Task DeleteProduct(int productId);
        Task<Product> GetProductBySearchName(string prefix);
        
    }
}
