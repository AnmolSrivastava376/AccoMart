using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Data.Models.DTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.Controllers.Order
{


    [ApiController]
    [Route("OrderController")]
    public class OrderController : ControllerBase
    {
        private readonly string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
/*
        [Authorize]
        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder(ProductOrderDto order)
        {


            if (order == null)
            {
                return BadRequest("Invalid order data");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string fetchPriceQuery = "SELECT ProductPrice FROM Product WHERE ProductId = @ProductId";

                    using (SqlCommand fetchPriceCommand = new SqlCommand(fetchPriceQuery, connection))
                    {
                        fetchPriceCommand.Parameters.AddWithValue("@ProductId", order.ProductId);
                        float productPrice = Convert.ToSingle(fetchPriceCommand.ExecuteScalar());

                        float orderAmount = productPrice;

                        string sqlQuery = @"INSERT INTO Orders (AddressId, UserId, ProductId, DeliveryServiceID, OrderAmount) 
                                VALUES (@AddressId, @UserId, @ProductId, @DeliveryServiceID, @OrderAmount);
                                SELECT SCOPE_IDENTITY();";

                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@AddressId", order.AddressId);
                            command.Parameters.AddWithValue("@UserId", order.UserId);
                            command.Parameters.AddWithValue("@ProductId", order.ProductId);
                            command.Parameters.AddWithValue("@DeliveryServiceID", order.DeliveryServiceID);
                            command.Parameters.AddWithValue("@OrderAmount", orderAmount);

                            command.ExecuteNonQuery();

                        }
                    }
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while placing the order: {ex.Message}");
            }
        }*/


        [Authorize]
        [HttpPost("PlaceOrderByCart")]
        public IActionResult PlaceOrderByCart(CartOrderDto orderr)
        {
            var user = HttpContext.User as ClaimsPrincipal;

            var userIdClaim = user.FindFirst("UserId");
            string userId="0";
            if (userIdClaim != null)
            {
                 userId = userIdClaim.Value;
            }

            // Get the CartId claim
            var cartIdClaim = user.FindFirst("CartId");
            int cartId=0;
            if (cartIdClaim != null)
            {

                 cartId = int.Parse(cartIdClaim.Value);

                // Use cartId as needed
            }

            try
            {
                // Create a connection to the database
                using (var connection = new SqlConnection(connectionString))
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
                        getDeliveryPriceCommand.Parameters.AddWithValue("@DServiceId", orderr.DeliveryServiceID);
                        DeliveryPrice = (decimal)getDeliveryPriceCommand.ExecuteScalar();
                    }


                    decimal TotalAmount;
                    TotalAmount = ProductAmount+ DeliveryPrice; 


                    


                        // Insert the order into the database
                            string insertOrderQuery = @"
                    INSERT INTO Orders (UserId, AddressId, CartId, DeliveryServiceID, OrderAmount) 
                    VALUES (@UserId, @AddressId, @CartId, @DeliveryServiceID, @OrderAmount);
                    SELECT SCOPE_IDENTITY();";

                    using (var insertOrderCommand = new SqlCommand(insertOrderQuery, connection))
                    {
                        insertOrderCommand.Parameters.AddWithValue("@UserId", userId);
                        insertOrderCommand.Parameters.AddWithValue("@AddressId", orderr.AddressId);
                        insertOrderCommand.Parameters.AddWithValue("@CartId", cartId);
                        insertOrderCommand.Parameters.AddWithValue("@DeliveryServiceID", orderr.DeliveryServiceID);
                        insertOrderCommand.Parameters.AddWithValue("@OrderAmount", TotalAmount);

                        int newOrderId = Convert.ToInt32(insertOrderCommand.ExecuteScalar());

                        return Ok(newOrderId);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while placing the order: {ex.Message}");
            }
        }


        [HttpPut("CancelOrder/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE [Order] SET isCancelled = 1 WHERE orderId = @orderId";

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
