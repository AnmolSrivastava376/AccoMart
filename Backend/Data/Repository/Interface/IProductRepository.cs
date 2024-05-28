using Data.Models;
using Data.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Data.Repository.Interfaces
{
    public interface IProductRepository
    {

        Task<Models.Product> GetProductById(int id);
        Task<Category> GetCategoryByName(string name);
        Task<List<Models.Product>> GetAllProductsByCategoryAsync(int id,string orderBy);
        Task<List<Models.Product>> GetProductsByPageNoAsync(int id, int pageNo, int pageSize);
        Task<List<Models.Product>> GetAllProductsAsync();
        Task<List<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int id);
        Task<Category> CreateCategory(string categoryName);
        Task<Models.Product> CreateProduct(Models.DTO.Product productDto);
        Task<Category> UpdateCategory(int Id, string NewCategoryName);
        Task<Models.Product> UpdateProduct(int productId, UpdateProduct productDto);
        Task DeleteCategory(int CategoryId);
        Task DeleteProduct(int productId);
        Task<List<Models.Product>> GetProductBySearchName(string prefix);
        Task<List<Models.Product>> GetProductsByCategoryName(string name);

        

    }
}
