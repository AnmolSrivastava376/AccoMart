using Data.Models.Product_Category.Category;
using Data.Models.Product_Category.Product;
using Data.Models.ViewModels;
using Data.Models.ViewModels.UpdateProduct;

namespace Data.Repository.Interfaces
{
    public interface IProductRepository
    {

        Task<Product> GetProductById(int id, string userId);
        Task<Category> GetCategoryByName(string name, string userId);
        Task<List<Product>> GetAllProductsByCategory(int id,string orderBy);
        Task<List<Product>> GetAllProductsPagewise(int pageNo, int pageSize, string userId);
        Task<List<Product>> GetProductsByPageNo(int id, int pageNo, int pageSize);
        Task<List<Product>> GetAllProducts();
        Task<List<Category>> GetAllCategories();
        Task<List<Category>> GetAllCategoriesAdmin(string userId);
        Task<Category> GetCategoryById(int id,string userId);
        Task<Category> CreateCategory(string categoryName);
        Task<Product> CreateProduct(ViewProduct productDto,string userId);
        Task<Category> UpdateCategory(int Id, string NewCategoryName);
        Task<Product> UpdateProduct(int productId, UpdateProduct productDto);
        Task DeleteCategory(int CategoryId);
        Task DeleteProduct(int productId);
        Task<List<Product>> GetProductBySearchName(string prefix);
        Task<List<Product>> GetProductBySearchNameAdmin(string userId, string prefix);
        Task<List<Product>> GetProductsByCategoryName(string name);      

    }
}
