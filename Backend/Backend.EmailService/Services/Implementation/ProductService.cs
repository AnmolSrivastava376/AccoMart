using Data.Models.Product_Category.Category;
using Data.Models.Product_Category.Product;
using Data.Models.ViewModels;
using Data.Models.ViewModels.UpdateProduct;
using Data.Repository.Interfaces;
using Service.Services.Interface;

namespace Service.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        async Task<Category> IProductService.CreateCategoryAsync(string categoryName, string userId)
        {
            return await _productRepository.CreateCategory(categoryName, userId);
        }

        async Task<Product> IProductService.CreateProductAsync(ViewProduct productDto,string userId)
        {
            return await _productRepository.CreateProduct(productDto,userId);
        }
       

        async Task IProductService.DeleteCategoryAsync(int CategoryId)
        {
            await _productRepository.DeleteCategory(CategoryId);
        }

        async Task IProductService.DeleteProductAsync(int ProductId)
        {
            await _productRepository.DeleteProduct(ProductId);
        }
        
        async Task<List<Category>> IProductService.GetAllCategoriesAsync()
        {
            return await _productRepository.GetAllCategories();
        }

        async Task<List<Product>> IProductService.GetAllProductsByCategoryAsync(int id, string orderBy)
        {
            return await _productRepository.GetAllProductsByCategory(id, orderBy);
        }

        async Task<List<Product>> IProductService.GetAllProductsAsync()
        {
            return await _productRepository.GetAllProducts();
        }
        async Task<List<Product>> IProductService.GetAllProductsPagewiseAsync(int pageNo, int pageSize, string userId)
        {
            return await _productRepository.GetAllProductsPagewise(pageNo, pageSize, userId);
        }

        async Task<List<Product>> IProductService.GetProductsByPageNoAsync(int id, int pageNo, int pageSize)
        {
            return await _productRepository.GetProductsByPageNo(id, pageNo, pageSize);
        }
        async Task<Category> IProductService.GetCategoryByIdAsync(int id,string userId)
        {
            return await _productRepository.GetCategoryById(id,userId);
        }

        async Task<Category> IProductService.GetCategoryByNameAsync(string name, string userId)
        {
            return await _productRepository.GetCategoryByName(name, userId);
        }




        async Task<Product> IProductService.GetProductByIdAsync(int id,string userId)
        {
            return await _productRepository.GetProductById(id,userId);
        }


        async Task<List<Product>> IProductService.GetProductBySearchNameAsync(string prefix)
        {
            return await _productRepository.GetProductBySearchName(prefix);
        }


        
     async Task<List<Product>> IProductService.GetProductsByCategoryNameAsync(string name)
        {
            return await _productRepository.GetProductsByCategoryName(name);
        }


        async Task<Category> IProductService.UpdateCategoryAsync(int Id, string NewCategoryName)
        {
            return await _productRepository.UpdateCategory(Id, NewCategoryName);
        }

        async Task<Product> IProductService.UpdateProductAsync(int productId, UpdateProduct productDto)
        {
            return await _productRepository.UpdateProduct(productId, productDto);
        }

        async Task<List<Category>> IProductService.GetAllCategoriesAdminAsync(string userId)
        {
            return await _productRepository.GetAllCategoriesAdmin(userId);
        }

        async Task<List<Product>> IProductService.GetProductBySearchNameAdminAsync(string userId, string prefix)
        {
            return await _productRepository.GetProductBySearchNameAdmin(userId,prefix);
        }
    }
}
