﻿using Data.Models;
using Data.Models.Statistic_Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Service.Services.Interface;

namespace API.Controllers.Chart
{
    [ApiController]
    [Route("ChartController")]
    public class ChartController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly ICartService _cartService;
        private readonly string _domain = "http://localhost:4200/";

        public ChartController(IConfiguration configuration, ICartService cartService)
        {
            _configuration = configuration;
            _cartService = cartService;
            _connectionString = _configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"];
        }

        [HttpGet("FetchDailyOrderQuantity")]
        public async Task<IActionResult> FetchDailyOrderQuantity()
        {
            try
            {
                List<OrderQuantity> orderQuantity = new List<OrderQuantity>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string getAllOrdersQuery = "SELECT o.OrderId, o.ProductId, o.Quantity, od.OrderDate, p.ProductPrice " +
                                               "FROM OrderHistory o " +
                                               "INNER JOIN Orders od ON o.OrderId = od.OrderId " +
                                               "INNER JOIN Product p ON o.ProductId = p.ProductId";
                    using (SqlCommand command = new SqlCommand(getAllOrdersQuery, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int quantity = Convert.ToInt32(reader["Quantity"]);
                                DateTime orderDate = Convert.ToDateTime(reader["OrderDate"]);
                                long productPrice = Convert.ToInt64(reader["ProductPrice"]);

                                OrderQuantity obj = orderQuantity.FirstOrDefault(x => x.OrderDate.Date == orderDate.Date);
                                if (obj != null)
                                {
                                    obj.Count += quantity;
                                    obj.TotalSales += quantity * productPrice;
                                }
                                else
                                {
                                    orderQuantity.Add(new OrderQuantity
                                    {
                                        OrderDate = orderDate,
                                        Count = quantity,
                                        TotalSales = quantity * productPrice
                                    });
                                }
                            }
                        }
                    }
                }
                return Ok(orderQuantity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("FetchCategoryWiseQuantity")]
        public async Task<IActionResult> FetchCategoryWiseQuantity()
        {
            try
            {
                List<CategoryItem> categoryItems = new List<CategoryItem>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string getAllOrdersQuery = "SELECT o.OrderId, o.ProductId, o.Quantity, c.CategoryId, c.CategoryName " +
                                               "FROM OrderHistory o " +
                                               "INNER JOIN Product p ON o.ProductId = p.ProductId " +
                                               "INNER JOIN Category c ON p.CategoryId = c.CategoryId";
                    using (SqlCommand command = new SqlCommand(getAllOrdersQuery, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int quantity = Convert.ToInt32(reader["Quantity"]);
                                int categoryId = Convert.ToInt32(reader["CategoryId"]);
                                string categoryName = Convert.ToString(reader["CategoryName"]);

                                CategoryItem obj = categoryItems.FirstOrDefault(x => x.CategoryName == categoryName) ;
                                if(obj!= null)
                                {
                                    obj.Quantity += quantity;
                                    obj.CategoryId = categoryId;
                                }
                                else
                                {
                                    categoryItems.Add(new CategoryItem
                                    {
                                        Quantity = quantity,
                                        CategoryName = categoryName,
                                        CategoryId = categoryId,
                                    });
                                }
                            }
                        }
                    }
                }
                return Ok(categoryItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("FetchProductWiseQuantity")]
        public async Task<IActionResult> FetchProductWiseQuantity()
        {
            try
            {
                List<ProductItem> productItems = new List<ProductItem>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string getAllOrdersQuery = "SELECT o.OrderId, o.ProductId, o.Quantity, p.ProductName " +
                                               "FROM OrderHistory o " +
                                               "INNER JOIN Product p ON o.ProductId = p.ProductId";
                    using (SqlCommand command = new SqlCommand(getAllOrdersQuery, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int quantity = Convert.ToInt32(reader["Quantity"]);
                                int productId = Convert.ToInt32(reader["ProductId"]);
                                string productName = Convert.ToString(reader["ProductName"]);

                                ProductItem obj = productItems.FirstOrDefault(x => x.ProductName == productName);
                                if (obj != null)
                                {
                                    obj.Quantity += quantity;
                                    obj.ProductId = productId;
                                }
                                else
                                {
                                    productItems.Add(new ProductItem
                                    {
                                        Quantity = quantity,
                                        ProductName = productName,
                                        ProductId = productId,
                                    });
                                }
                            }
                        }
                    }
                }
                return Ok(productItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
