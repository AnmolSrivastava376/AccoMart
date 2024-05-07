using Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Service.Services.Interface;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers.Order
{
    [ApiController]
    [Route("OrderController")]
    public class OrderController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly ICartService _cartService;
        private readonly string _domain = "https://localhost:7153/Home/Index";

        public OrderController(IConfiguration configuration, ICartService cartService)
        {
            _configuration = configuration;
            _cartService = cartService;
            _connectionString = _configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"];
        }

        [Authorize]
        [HttpPost("PlaceOrderByProduct")]
        public async Task<IActionResult> PlaceOrder(ProductOrderDto order)
        {
            if (order == null)
            {
                return BadRequest("Invalid order data");
            }

            try
            {
                int orderId = 0;
                var user = HttpContext.User as ClaimsPrincipal;
                var userIdClaim = user.FindFirst("UserId");
                string userId = userIdClaim?.Value ?? "0";

                var cartIdClaim = user.FindFirst("CartId");
                int cartId = cartIdClaim != null ? int.Parse(cartIdClaim.Value) : 0;

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    decimal deliveryPrice;
                    string deliveryPriceQuery = "SELECT Price FROM DeliveryService WHERE DServiceId = @DServiceId";
                    using (SqlCommand deliveryPriceCommand = new SqlCommand(deliveryPriceQuery, connection))
                    {
                        deliveryPriceCommand.Parameters.AddWithValue("@DServiceId", order.DeliveryServiceID);
                        deliveryPrice = (decimal)await deliveryPriceCommand.ExecuteScalarAsync();
                    }

                    string fetchPriceQuery = "SELECT ProductPrice FROM Product WHERE ProductId = @ProductId";

                    using (SqlCommand fetchPriceCommand = new SqlCommand(fetchPriceQuery, connection))
                    {
                        fetchPriceCommand.Parameters.AddWithValue("@ProductId", order.ProductId);
                        decimal productPrice = (decimal)await fetchPriceCommand.ExecuteScalarAsync();
                        decimal orderAmount = productPrice + deliveryPrice;

                        string sqlQuery = @"INSERT INTO Orders (AddressId, UserId, ProductId, DeliveryServiceID, OrderAmount)
                                            VALUES (@AddressId, @UserId, @ProductId, @DeliveryServiceID, @OrderAmount);
                                            SELECT SCOPE_IDENTITY();";

                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@AddressId", order.AddressId);
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@ProductId", order.ProductId);
                            command.Parameters.AddWithValue("@DeliveryServiceID", order.DeliveryServiceID);
                            command.Parameters.AddWithValue("@OrderAmount", orderAmount);
                            orderId = Convert.ToInt32(await command.ExecuteScalarAsync());
                        }
                    }
                }

                await _cartService.GenerateInvoiceAsync(orderId);
                await _cartService.DeleteCartAsync(cartId);
                return await Checkout();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while placing the order: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("PlaceOrderByCart")]
        public async Task<IActionResult> PlaceOrderByCart(CartOrderDto order)
        {
            var user = HttpContext.User as ClaimsPrincipal;
            int newOrderId = 0;

            var userIdClaim = user.FindFirst("UserId");
            string userId = userIdClaim?.Value ?? "0";

            var cartIdClaim = user.FindFirst("CartId");
            int cartId = cartIdClaim != null ? int.Parse(cartIdClaim.Value) : 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    decimal productAmount = 0.00M;
                    string getProductAmountQuery = @"
                        SELECT CAST(SUM(p.ProductPrice * ci.Quantity) AS DECIMAL(18,2)) AS TotalAmount
                        FROM CartItem ci
                        JOIN Product p ON ci.ProductId = p.ProductId
                        WHERE ci.CartId = @CartId";

                    using (var getProductAmountCommand = new SqlCommand(getProductAmountQuery, connection))
                    {
                        getProductAmountCommand.Parameters.AddWithValue("@CartId", cartId);
                        object result = await getProductAmountCommand.ExecuteScalarAsync();

                        if (result != DBNull.Value)
                        {
                            productAmount = (decimal)result;
                        }
                    }

                    decimal deliveryPrice;
                    string getDeliveryPriceQuery = @"
                        SELECT Price
                        FROM DeliveryService
                        WHERE DServiceId = @DServiceId;";

                    using (var getDeliveryPriceCommand = new SqlCommand(getDeliveryPriceQuery, connection))
                    {
                        getDeliveryPriceCommand.Parameters.AddWithValue("@DServiceId", order.DeliveryServiceID);
                        deliveryPrice = (decimal)await getDeliveryPriceCommand.ExecuteScalarAsync();
                    }

                    decimal totalAmount = productAmount + deliveryPrice;

                    string insertOrderQuery = @"
                        INSERT INTO Orders (UserId, AddressId, CartId, DeliveryServiceID, OrderAmount) 
                        VALUES (@UserId, @AddressId, @CartId, @DeliveryServiceID, @OrderAmount);
                        SELECT SCOPE_IDENTITY();";

                    using (var insertOrderCommand = new SqlCommand(insertOrderQuery, connection))
                    {
                        insertOrderCommand.Parameters.AddWithValue("@UserId", userId);
                        insertOrderCommand.Parameters.AddWithValue("@AddressId", order.AddressId);
                        insertOrderCommand.Parameters.AddWithValue("@CartId", cartId);
                        insertOrderCommand.Parameters.AddWithValue("@DeliveryServiceID", order.DeliveryServiceID);
                        insertOrderCommand.Parameters.AddWithValue("@OrderAmount", totalAmount);

                        newOrderId = Convert.ToInt32(await insertOrderCommand.ExecuteScalarAsync());
                    }
                }

                await _cartService.GenerateInvoiceAsync(newOrderId);
                //await _cartService.DeleteCartAsync(cartId); // commented out as per your original code
                return await Checkout();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while placing the order: {ex.Message}");
            }
        }

        [HttpPost("Checkout/")]
        public async Task<IActionResult> Checkout()
        {
            var user = HttpContext.User as ClaimsPrincipal;
            var userEmailClaim = user.FindFirst("UserEmail");
            string userEmail = userEmailClaim?.Value ?? "0";

            var cartIdClaim = user.FindFirst("CartId");
            int cartId = cartIdClaim != null ? int.Parse(cartIdClaim.Value) : 0;

            var options = new SessionCreateOptions
            {
                SuccessUrl = _domain + "Checkout/OrderConfirmation",
                CancelUrl = _domain + "Cart/GetCart",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "sdfgh@gmail.com"
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string getCartItemQuery = @"SELECT * FROM CartItem WHERE CartId = @CartId";

                using (var getCartItemCommand = new SqlCommand(getCartItemQuery, connection))
                {
                    getCartItemCommand.Parameters.AddWithValue("@CartId", cartId);
                    object result = await getCartItemCommand.ExecuteScalarAsync();

                    if (result != DBNull.Value)
                    {
                        using (SqlDataReader cartItemReader = await getCartItemCommand.ExecuteReaderAsync())
                        {
                            while (await cartItemReader.ReadAsync())
                            {
                                int productId = Convert.ToInt32(cartItemReader["ProductId"]);

                                string getProductQuery = @"SELECT * FROM Product WHERE ProductId = @ProductId";

                                using (var getProductCommand = new SqlCommand(getProductQuery, connection))
                                {
                                    getProductCommand.Parameters.AddWithValue("@ProductId", productId);
                                     //await getProductCommand.ExecuteNonQueryAsync();

                                   
                                       
                                            while (await cartItemReader.ReadAsync())
                                            {
                                        decimal productPrice = (decimal)cartItemReader["ProductPrice"];
                                        var sessionListItem = new SessionLineItemOptions
                                                {
                                                    PriceData = new SessionLineItemPriceDataOptions
                                                    {
                                                        UnitAmount = (long)(productPrice * 100),
                                                        Currency = "inr",
                                                        ProductData = new SessionLineItemPriceDataProductDataOptions
                                                        {
                                                            Name = GetProductName(Convert.ToInt32(cartItemReader["ProductId"]))
                                                        }
                                                    },
                                                    Quantity = (long)Convert.ToInt32(cartItemReader["Quantity"])
                                                };

                                                options.LineItems.Add(sessionListItem);
                                            }
                                        
                                    
                                }
                            }
                        }
                    }
                }
            }

            var service = new SessionService();
            Session session = service.Create(options);
            HttpContext.Session.SetString("Session", session.Id);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [HttpGet("OrderConfirmation")]
        public IActionResult OrderConfirmation()
        {
            var service = new SessionService();
            Session session = new Session();
            HttpContext.Session.GetString("Session");
            using var httpClient = new HttpClient();
            if (session.PaymentStatus == "Paid")
            {
                var response = httpClient.GetAsync(_domain);

                if (response.IsCompletedSuccessfully)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return BadRequest();
        }

        private string GetProductName(int productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string getProductQuery = "SELECT ProductName FROM Product WHERE Id = @ProductId";
                using (var getProductCommand = new SqlCommand(getProductQuery, connection))
                {
                    getProductCommand.Parameters.AddWithValue("@ProductId", productId);
                    return getProductCommand.ExecuteScalar()?.ToString() ?? "Unknown Product";
                }
            }
        }

        [HttpPut("CancelOrder/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE [Orders] SET isCancelled = 1 WHERE orderId = @orderId";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@orderId", orderId);
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return NotFound("Order not found.");
                        }
                    }
                }

                return Ok("Order has been cancelled successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
