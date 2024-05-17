using Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Service.Services.Interface;
using Stripe;
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
        private readonly string _domain = "http://localhost:4200/";

        public OrderController(IConfiguration configuration, ICartService cartService)
        {
            _configuration = configuration;
            _cartService = cartService;
            _connectionString = _configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"];
        }

        [HttpPost("PlaceOrderByCart")]
        public async Task<string> PlaceOrderByCart(string userId, int cartId, int addressId, int deliveryId)
        {
            int newOrderId = 0;

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
                        getDeliveryPriceCommand.Parameters.AddWithValue("@DServiceId", deliveryId);
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
                        insertOrderCommand.Parameters.AddWithValue("@AddressId", addressId);
                        insertOrderCommand.Parameters.AddWithValue("@CartId", cartId);
                        insertOrderCommand.Parameters.AddWithValue("@DeliveryServiceID", deliveryId);
                        insertOrderCommand.Parameters.AddWithValue("@OrderAmount", totalAmount);

                        newOrderId = Convert.ToInt32(await insertOrderCommand.ExecuteScalarAsync());
                    }
                }

                return await CheckoutByCart(userId, cartId);
            }
            catch (Exception ex)
            {
                return $"An error occurred while placing the order";
            }
        }

        //[Authorize]
        [HttpPost("Checkout/Cart")]
        public async Task<string> CheckoutByCart(string userId, int cartId)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = _domain + "home/yourorders",
                CancelUrl = _domain + "home/cart",
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

                    using (SqlDataReader cartItemReader = await getCartItemCommand.ExecuteReaderAsync())
                    {
                        List<Tuple<int, int>> CartItems = new List<Tuple<int, int>>();

                        while (await cartItemReader.ReadAsync())
                        {
                            int productId = Convert.ToInt32(cartItemReader["ProductId"]);
                            int productQuantity = Convert.ToInt32(cartItemReader["Quantity"]);

                            CartItems.Add(Tuple.Create(productId, productQuantity));
                        }

                        await cartItemReader.CloseAsync();

                        foreach (Tuple<int, int> CartItem in CartItems)
                        {
                            string getProductQuery = @"SELECT * FROM Product WHERE ProductId = @ProductId";

                            using (var getProductCommand = new SqlCommand(getProductQuery, connection))
                            {
                                getProductCommand.Parameters.AddWithValue("@ProductId", CartItem.Item1);

                                using (SqlDataReader productReader = await getProductCommand.ExecuteReaderAsync())
                                {
                                    while (await productReader.ReadAsync())
                                    {
                                        string name = Convert.ToString(productReader["ProductName"]);
                                        decimal productPrice = (decimal)productReader["ProductPrice"];

                                        var sessionListItem = new SessionLineItemOptions
                                        {
                                            PriceData = new SessionLineItemPriceDataOptions
                                            {
                                                UnitAmount = (long)(productPrice * 100),
                                                Currency = "inr",
                                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                                {
                                                    Name = name
                                                }
                                            },
                                            Quantity = CartItem.Item2
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
            return session.Url;
        }

        //[Authorize]
        [HttpPost("PlaceOrderByProduct")]
        public async Task<string> PlaceOrder(string userId, int addressId, int deliveryId, int productId)
        {
          
            try
            {
                int orderId = 0;
                /*var user = HttpContext.User as ClaimsPrincipal;
                var userIdClaim = user.FindFirst("UserId");
                string userId = userIdClaim?.Value ?? "0";

                var cartIdClaim = user.FindFirst("CartId");
                int cartId = cartIdClaim != null ? int.Parse(cartIdClaim.Value) : 0;*/

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    decimal deliveryPrice;
                    string deliveryPriceQuery = "SELECT Price FROM DeliveryService WHERE DServiceId = @DServiceId";
                    using (SqlCommand deliveryPriceCommand = new SqlCommand(deliveryPriceQuery, connection))
                    {
                        deliveryPriceCommand.Parameters.AddWithValue("@DServiceId", deliveryId);
                        deliveryPrice = (decimal)await deliveryPriceCommand.ExecuteScalarAsync();
                    }

                    string fetchPriceQuery = "SELECT ProductPrice FROM Product WHERE ProductId = @ProductId";

                    using (SqlCommand fetchPriceCommand = new SqlCommand(fetchPriceQuery, connection))
                    {
                        fetchPriceCommand.Parameters.AddWithValue("@ProductId", productId);
                        decimal productPrice = (decimal)await fetchPriceCommand.ExecuteScalarAsync();
                        decimal orderAmount = productPrice + deliveryPrice;

                        string sqlQuery = @"INSERT INTO Orders (AddressId, UserId, ProductId, DeliveryServiceID, OrderAmount)
                                            VALUES (@AddressId, @UserId, @ProductId, @DeliveryServiceID, @OrderAmount);
                                            SELECT SCOPE_IDENTITY();";

                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@AddressId", addressId);
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@ProductId", productId);
                            command.Parameters.AddWithValue("@DeliveryServiceID", deliveryId);
                            command.Parameters.AddWithValue("@OrderAmount", orderAmount);
                            orderId = Convert.ToInt32(await command.ExecuteScalarAsync());
                        }
                    }
                }

                //await _cartService.GenerateInvoiceAsync(orderId);
                //await _cartService.DeleteCartAsync(cartId);
                return await Checkout(productId);
            }
            catch (Exception ex)
            {
                return "An error occurred while placing the order";
            }
        }

   

        //[Authorize]
        [HttpPost("Checkout/Product")]
        public async Task<string> Checkout(int productId)
        {

            var options = new SessionCreateOptions
            {
                SuccessUrl = _domain + "home/yourorders",
                CancelUrl = _domain + "home/buy-product/:productId",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "sdfgh@gmail.com",

            };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
               
                    string getProductQuery = @"SELECT * FROM Product WHERE ProductId = @ProductId";

                        using (var getProductCommand = new SqlCommand(getProductQuery, connection))
                        {
                            getProductCommand.Parameters.AddWithValue("@ProductId", productId);
                             await getProductCommand.ExecuteNonQueryAsync();


                    using (SqlDataReader cartItemReader = await getProductCommand.ExecuteReaderAsync())
                    {
                        while (await cartItemReader.ReadAsync())
                        {
                            string name = Convert.ToString(cartItemReader["ProductName"]);
                            string newName = name;
                            decimal productPrice = (decimal)cartItemReader["ProductPrice"];

                            var sessionListItem = new SessionLineItemOptions
                            {
                                PriceData = new SessionLineItemPriceDataOptions
                                {
                                    UnitAmount = (long)(productPrice * 100),
                                    Currency = "inr",
                                    ProductData = new SessionLineItemPriceDataProductDataOptions
                                    {
                                        Name = name
                                    }
                                },
                                Quantity = (long)1
                            };

                            options.LineItems.Add(sessionListItem);
                        }


                    }
                            
                      
                    
                }
            }

            var service = new SessionService();
            Session session = service.Create(options);
            HttpContext.Session.SetString("Session", session.Id);
            Response.Headers.Add("Location", session.Url);
            return session.Url;
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
