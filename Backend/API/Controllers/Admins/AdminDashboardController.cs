﻿using Data.Models;
using Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;

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


        [HttpGet("Products/SearchBy={prefix}")]
        public async Task<List<Product>> GetProductBySearchName(string prefix = "")
        {
            return await _productService.GetProductBySearchNameAsync(prefix);  
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

        [Authorize(Roles = "Admin")]
        [HttpPost("Category/Create")]
         public async Task<ActionResult<Category>> CreateCategory([FromBody] CategoryName category_name)
         {

            var category = await _productService.CreateCategoryAsync(category_name.name);
            return Ok(category);    
         }


        [Authorize(Roles = "Admin")]
        [HttpPost("Product/Create")]
        public async Task<ActionResult<Product>> CreateProduct(ProductDto productDto)
        {
            var product =   await _productService.CreateProductAsync(productDto);
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

        async public Task<ActionResult<Product>> UpdateProduct (int productId, UpdateProductDto productDto)
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
