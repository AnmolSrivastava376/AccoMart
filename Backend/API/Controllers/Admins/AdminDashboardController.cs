﻿using API.Models;
using API.Models.DTO;
using API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.Controllers.Admins
{
    // [Authorize]
    [Route("AdminDashboard")]
    [ApiController]
    public class AdminDashboardController : Controller
    {
        private readonly IProductService _productService;
        public AdminDashboardController(IProductService productService)
        {
            _productService = productService;
        }
        
        [HttpGet("Products/CategoryId")]
        public async Task<IEnumerable<Product>> GetAllProducts(int id)
        {
           
            return await _productService.GetAllProductsAsync(id); 
        }

        [HttpGet("Product/ProductId")]
         public async Task<Product> GetProductById(int id)
        {

            return await _productService.GetProductByIdAsync(id);
        }

        [HttpGet("GetAllCategories")]
        async public Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _productService.GetAllCategoriesAsync();
        }

       
       [HttpGet("Category/CategoryId")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _productService.GetCategoryByIdAsync(id);
            return Ok(category);
        }


        [HttpPost("Category/Create")]
         public async Task<ActionResult<Category>> CreateCategory(string categoryName)
         {

            var category = await _productService.CreateCategoryAsync(categoryName);
            return Ok(category);    
         }

        [HttpPost("Product/Create")]
        public async Task<ActionResult<Product>> CreateProduct(ProductDto productDto)
        {
            var product =   await _productService.CreateProductAsync(productDto);
            return Ok(product); 
        }


        [HttpPut("Update/Category")]
        async public Task<ActionResult<Category>> UpdateCategory(string categoryName, string NewCategoryName)
        {
           var category = await _productService.UpdateCategoryAsync(categoryName, NewCategoryName); 
           return  Ok(category);   
        }

        [HttpPut("Update/Product")]

        async public Task<ActionResult<Product>> UpdateProduct (int productId, UpdateProductDto productDto)
        {
           var product = await _productService.UpdateProductAsync(productId, productDto);
            return Ok(product);
        }


        [HttpDelete("Delete/Category")]
        async public Task<ActionResult> DeleteCategory(int CategoryId)
        {
            await _productService.DeleteCategoryAsync(CategoryId);
            return Ok();
        }




        [HttpDelete("Delete/Product")]
        async public Task<ActionResult> DeleteProduct(int ProductId)
        {
            await _productService.DeleteProductAsync(ProductId);
            return Ok();
        }





    }
}
