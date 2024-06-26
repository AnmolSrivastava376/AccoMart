﻿using Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Data.Models.Product_Category.Product;
using Data.Models.Product_Category.Category;
using Data.Models.ViewModels;
using Data.Models.Product_Category;
using Data.Models.ViewModels.UpdateProduct;

namespace API.Controllers.Admins
{
    [Route("AdminDashboard")]
    [ApiController]
    public class AdminDashboardController : Controller
    {
        private readonly IProductService _productService;
        public AdminDashboardController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("Products/CategoryId={id}")]
        public async Task<List<Product>> GetAllProducts(int id, string orderBy)
        {
            return await _productService.GetAllProductsByCategoryAsync(id, orderBy);
        }
        [HttpGet("ProductsByPageNo")]
        public async Task<List<Product>> GetProductsByPageNo(int id, int pageNo)
        {
            const int pageSize = 10;
            return await _productService.GetProductsByPageNoAsync(id, pageNo, pageSize);
        }
        [HttpGet("Products")]
        public async Task<List<Product>> GetProducts()
        {
            return await _productService.GetAllProductsAsync();
        }

        [HttpGet("GetAllProductsPagewise")]
        public async Task<List<Product>> GetAllProductsPagewise(int pageNo)
        {
            int pageSize = 20;
            return await _productService.GetAllProductsPagewiseAsync(pageNo,pageSize);
        }
        [HttpGet("Products/SearchBy={prefix}")]
        public async Task<List<Product>> GetProductBySearchName(string prefix = "")
        {
            return await _productService.GetProductBySearchNameAsync(prefix);
        }

        [HttpGet("Products/CategoryName={name}")]
        public async Task<List<Product>> GetProductsByCategoryName(string name = "")
        {
         return await _productService.GetProductsByCategoryNameAsync(name);
        }


        [HttpGet("Product/{id}")]
         public async Task<Product> GetProductById(int id)
        {

            return await _productService.GetProductByIdAsync(id);
        }

        [HttpGet("GetAllCategories")]
        async public Task<List<Category>> GetAllCategories()
        {
            return await _productService.GetAllCategoriesAsync();
        }

       
        [HttpGet("Category/{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _productService.GetCategoryByIdAsync(id);
            return Ok(category);
        }

        [HttpGet("Category/name/{name}")]
        public async Task<ActionResult<Category>> GetCategoryByName(string name)
        {
            var category = await _productService.GetCategoryByNameAsync(name);
            return Ok(category);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("Category/Create")]
         public async Task<ActionResult<Category>> CreateCategory([FromBody] CategoryName category_name)
         {

            var category = await _productService.CreateCategoryAsync(category_name.name);
            return Ok(category);    
         }


        [Authorize(Roles = "Admin")]
        [HttpPost("Product/Create")]
        public async Task<ActionResult<Product>> CreateProduct(ViewProduct productDto)
        {
            var product = await _productService.CreateProductAsync(productDto);
            return Ok(product); 
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update/Category")]
        async public Task<ActionResult<Category>> UpdateCategory(int Id, string NewCategoryName)
        {
           var category = await _productService.UpdateCategoryAsync( Id, NewCategoryName); 
           return  Ok(category);   
        }

        [Authorize(Roles = "Admin")]

        [HttpPut("Update/Product/{productId}")]

        async public Task<ActionResult<Product>> UpdateProduct (int productId, UpdateProduct productDto)
        {
           var product = await _productService.UpdateProductAsync(productId, productDto);
            return Ok(product);
        }

        [Authorize(Roles = "Admin")]

        [HttpDelete("Delete/Category/{CategoryId}")]
        async public Task<ActionResult> DeleteCategory(int CategoryId)
        {
            await _productService.DeleteCategoryAsync(CategoryId);
            return Ok();
        }
        [Authorize(Roles = "Admin")]

        [HttpDelete("Delete/Product/{ProductId}")]
        async public Task<ActionResult> DeleteProduct(int ProductId)
        {
            await _productService.DeleteProductAsync(ProductId);
            return Ok();
        }

    }
}
