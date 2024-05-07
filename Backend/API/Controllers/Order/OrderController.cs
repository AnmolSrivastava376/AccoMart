using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Stripe.Checkout;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Service.Services.Interface;

namespace API.Controllers.Order
{
    [ApiController]
    [Route("OrderController")]
    public class OrderController : ControllerBase
    {
        private readonly string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly IConfiguration _configuration;
        private readonly ICartService _cartService;
        public OrderController(IConfiguration configuration, ICartService cartService)
        {
            _configuration = configuration;
            _cartService = cartService;
        }

        [Authorize]
        [HttpPost("PlaceOrderByProduct")]
        public IActionResult PlaceOrder(ProductOrderDto order)
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
                string userId = "0";
                var cartIdClaim = user.FindFirst("CartId");
                int cartId = 0;
                if (cartIdClaim != null)
                {

                    cartId = int.Parse(cartIdClaim.Value);
                }
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    connection.Open();
                    
                    if (userIdClaim != null)
                    {
                        userId = userIdClaim.Value;
                    }

                    decimal DeliveryPrice;
                    string DeliveryPriceQuery = "Select Price FROM DeliveryService WHERE DServiceId = @DServiceId";
                    using (SqlCommand DeliveryPriceCommand = new SqlCommand(DeliveryPriceQuery, connection))
                    {
                        DeliveryPriceCommand.Parameters.AddWithValue("@DServiceId", order.DeliveryServiceID);
                         DeliveryPrice = (decimal)DeliveryPriceCommand.ExecuteScalar();
                    }

                    string fetchPriceQuery = "SELECT ProductPrice FROM Product WHERE ProductId = @ProductId";


                    using (SqlCommand fetchPriceCommand = new SqlCommand(fetchPriceQuery, connection))
                    {
                        fetchPriceCommand.Parameters.AddWithValue("@ProductId", order.ProductId);
                        decimal productPrice = (decimal)fetchPriceCommand.ExecuteScalar();
                        decimal orderAmount = productPrice+DeliveryPrice;
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
                            orderId = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                }
                _cartService.GenerateInvoiceAsync(orderId);
                _cartService.DeleteCartAsync(cartId);
                return Checkout();

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while placing the order: {ex.Message}");
            }
        }


        [Authorize]
                    [HttpPost("PlaceOrderByCart")]
                    public IActionResult PlaceOrderByCart(CartOrderDto order)
                    {
                        var user = HttpContext.User as ClaimsPrincipal;
                        int newOrderId=0;

                        var userIdClaim = user.FindFirst("UserId");
                        string userId = "0";
                        if (userIdClaim != null)
                        {
                            userId = userIdClaim.Value;
                        }

                        // Get the CartId claim
                        var cartIdClaim = user.FindFirst("CartId");
                        int cartId = 0;
                        if (cartIdClaim != null)
                        {

                            cartId = int.Parse(cartIdClaim.Value);

                            // Use cartId as needed
                        }

                        try
                        {
                            // Create a connection to the database
                            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                            {
                                connection.Open();


                                decimal ProductAmount = 0.00M;
                                string getProductAmountQuery = @"
                        SELECT CAST(SUM(p.ProductPrice * ci.Quantity) AS DECIMAL(18,2)) AS TotalAmount
                        FROM CartItem ci
                        JOIN Product p ON ci.ProductId = p.ProductId
                        WHERE ci.CartId = @CartId";

                                using (var getProductAmountCommand = new SqlCommand(getProductAmountQuery, connection))
                                {
                                    getProductAmountCommand.Parameters.AddWithValue("@CartId", cartId);
                                    object result = getProductAmountCommand.ExecuteScalar();

                                    if (result != DBNull.Value)
                                    {
                                        ProductAmount = (decimal)result;
                                    }

                                }

                                decimal DeliveryPrice;
                                string getDeliveryPriceQuery = @"
                    SELECT Price
                    FROM DeliveryService
                    WHERE DServiceId = @DServiceId;
                    ";

                                using (var getDeliveryPriceCommand = new SqlCommand(getDeliveryPriceQuery, connection))
                                {
                                    getDeliveryPriceCommand.Parameters.AddWithValue("@DServiceId", order.DeliveryServiceID);
                                    DeliveryPrice = (decimal)getDeliveryPriceCommand.ExecuteScalar();
                                }


                                decimal TotalAmount;
                                TotalAmount = ProductAmount + DeliveryPrice;


                                // Insert the order into the database
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
                                    insertOrderCommand.Parameters.AddWithValue("@OrderAmount", TotalAmount);

                                     newOrderId = Convert.ToInt32(insertOrderCommand.ExecuteScalar());
                       

                                }
                            }
                _cartService.GenerateInvoiceAsync(newOrderId);
                //_cartService.DeleteCartAsync(cartId);
                return Checkout();
                //return Ok(order);
            }
                        catch (Exception ex)
                        {
                            return StatusCode(500, $"An error occurred while placing the order: {ex.Message}");
                        }
                    }

                    [HttpPost("Checkout/")]
                    public IActionResult Checkout()
                    {
                        var user = HttpContext.User as ClaimsPrincipal;

                        var userEmailClaim = user.FindFirst("UserEmail");
                        string userEmail = "0";
                        if (userEmailClaim != null)
                        {
                            userEmail = userEmailClaim.Value;
                        }

                        var cartIdClaim = user.FindFirst("CartId");
                        int cartId = 0;
                        if (cartIdClaim != null)
                        {
                            cartId = int.Parse(cartIdClaim.Value);
                        }
                        var domain = "http://localhost:5135/";
                        var options = new SessionCreateOptions
                        {
                            SuccessUrl = domain + $"Checkout/OrderConfirmation",
                            CancelUrl = domain + $"Cart/GetCart",
                            LineItems = new List<SessionLineItemOptions>(),
                            Mode = "payment",
                            CustomerEmail = userEmail
                        };



            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string getCartItemQuery = @"
        SELECT *
        FROM CartItem                    
        WHERE CartId = @CartId";

                using (var getCartItemCommand = new SqlCommand(getCartItemQuery, connection))
                {
                    getCartItemCommand.Parameters.AddWithValue("@CartId", cartId);
                    object result = getCartItemCommand.ExecuteScalar();

                    if (result != DBNull.Value)
                    {
                        SqlDataReader reader = getCartItemCommand.ExecuteReader();

                        // Read data from the first result set (Cart table)
                        while (reader.Read())
                        {
                            int productId = Convert.ToInt32(reader["ProductId"]);

                            string getProductQuery = @"
                    SELECT *
                    FROM Product                    
                    WHERE ProductId = @ProductId";

                            using (var getProductCommand = new SqlCommand(getProductQuery, connection))
                            {
                                getProductCommand.Parameters.AddWithValue("@ProductId", productId);
                                object result1 = getProductCommand.ExecuteScalar();

                                if (result1 != DBNull.Value)
                                {
                                    SqlDataReader reader2 = getProductCommand.ExecuteReader();

                                    // Read data from the first result set (Cart table)
                                    while (reader2.Read())
                                    {
                                        var sessionListItem = new SessionLineItemOptions
                                        {
                                            PriceData = new SessionLineItemPriceDataOptions
                                            {
                                                UnitAmount = (long)Convert.ToDouble(reader2["ProductPrice"]),
                                                Currency = "inr",
                                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                                {
                                                    Name = GetProductName(Convert.ToInt32(reader2["ProductId"]))
                                                }
                                            },
                                            Quantity = (long)Convert.ToInt32(reader["Quantity"])
                                        };

                                        int id = 0;
                                        options.LineItems.Add(sessionListItem);
                                        var service = new SessionService();
                                        Session session = service.Create(options);
                                        HttpContext.Session.SetString("Session", session.Id);
                                        Response.Headers.Add("Location", session.Url);
                                        return new StatusCodeResult(303);
                                    }
                                }
                            }
                        }
                        reader.Close();
                    }
                }
            }


            return BadRequest();

                    }

                    [HttpGet("OrderConfirmation")]
                    public IActionResult OrderConfirmation()
                    {
                        var service = new SessionService();
                        Session session = new Session();
                        HttpContext.Session.GetString("Session");           
                        if (session.PaymentStatus == "Paid")
                        {
                            return Ok();
                        }
                        return BadRequest();
                    }

                    private string GetProductName(int productId)
                    {

                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string getProductQuery = "SELECT ProductName FROM Product WHERE Id = @ProductId";
                            using (var getProductCommand = new SqlCommand(getProductQuery, connection))
                            {
                                getProductCommand.Parameters.AddWithValue("@ProductId", productId);
                                return getProductCommand.ExecuteScalar()?.ToString() ?? "Unknown Product";
                            }
                        }
                        return "Unknown Product";
                    }



                    [HttpPut("CancelOrder/{orderId}")]
                    public async Task<IActionResult> CancelOrder(int orderId)
                    {
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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
        
