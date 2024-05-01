﻿using API.Models.DTO;
using API.Models;
using API.Repository.Interfaces;
using API.Repository;

namespace API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        async Task<Category> IProductService.CreateCategoryAsync(string categoryName)
        {
            return await _productRepository.CreateCategory(categoryName);   
        }

        async Task<Product> IProductService.CreateProductAsync(ProductDto productDto)
        {
            string categoryName = productDto.CategoryName;
            var category = await _productRepository.GetCategoryByName(categoryName);
            int CategoryId = category.CategoryId;
            return await _productRepository.CreateProduct(productDto, CategoryId);
        }

       

        async Task IProductService.DeleteCategoryAsync(int CategoryId)
        {
            await _productRepository.DeleteCategory(CategoryId);
        }

        async Task IProductService.DeleteProductAsync(int ProductId)
        {
            await _productRepository.DeleteProduct(ProductId);
        }

        async Task<IEnumerable<Category>> IProductService.GetAllCategoriesAsync()
        {
            return await _productRepository.GetAllCategories();   
        }

        async Task<IEnumerable<Product>> IProductService.GetAllProductsAsync(int id)
        {
            return await _productRepository.GetAllProducts(id);
        }

        async Task<Category> IProductService.GetCategoryByIdAsync(int id)
        {
            return await _productRepository.GetCategoryById(id);
        }

        async Task<Product> IProductService.GetProductByIdAsync(int id)
        {
            return await _productRepository.GetProductById(id);
        }

        async Task<Category> IProductService.UpdateCategoryAsync(string categoryName, string NewCategoryName)
        {
            return await _productRepository.UpdateCategory(categoryName, NewCategoryName);  
        }

        async Task<Product> IProductService.UpdateProductAsync(int productId, UpdateProductDto productDto)
        {
            return await _productRepository.UpdateProduct(productId, productDto);
        }
    }
}