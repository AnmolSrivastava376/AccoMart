using API.Models;
using API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.Repository.Interfaces
{
    public interface IProductRepository
    {

        Task<Product> GetProductById(int id);
        Task<Category> GetCategoryByName(string name);
        Task<IEnumerable<Product>> GetAllProducts(int id);
        Task<IQueryable<IEnumerable<Category>>> GetAllCategories(string prefix);
        Task<Category> GetCategoryById(int id);
        Task<Category> CreateCategory(string categoryName);
        Task<Product> CreateProduct(ProductDto productDto, int categoryId);
        Task<Category> UpdateCategory(string categoryName, string NewCategoryName);
        Task<Product> UpdateProduct(int productId, UpdateProductDto productDto);
        Task DeleteCategory(int CategoryId);
        Task DeleteProduct(int productId);
        Task<Product> GetProductBySearchName(string prefix);
    }
}
