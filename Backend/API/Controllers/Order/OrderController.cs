using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using API.Models.DTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.Controllers.Order
{


    [ApiController]
    [Route("OrderController")]
    public class OrderController : ControllerBase
    {
        private readonly string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

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
        }


        [HttpPost("PlaceOrderByCart")]
        public IActionResult PlaceOrderByCart(CartOrderDto orderr,int userId)
        {
            try
            {
                // Create a connection to the database
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve the UserId associated with the CartId
                /*    string getUserIdQuery = "SELECT UserId FROM Cart WHERE CartId = @CartId";
                    using (var getUserIdCommand = new SqlCommand(getUserIdQuery, connection))
                    {
                        getUserIdCommand.Parameters.AddWithValue("@CartId", orderr.CartId);
                        userId = (int)getUserIdCommand.ExecuteScalar();
                    }*/

                    // Calculate the total amount of the cart items

                    decimal ProductAmount;
                    string getProductAmountQuery = @"
                        SELECT CAST(SUM(p.ProductPrice * ci.Quantity) AS DECIMAL(18,2)) AS TotalAmount
                        FROM CartItem ci
                        JOIN Product p ON ci.ProductId = p.ProductId
                        WHERE ci.CartId = @CartId";

                    using (var getProductAmountCommand = new SqlCommand(getProductAmountQuery, connection))
                    {
                        getProductAmountCommand.Parameters.AddWithValue("@CartId", orderr.CartId);
                        ProductAmount = (decimal)getProductAmountCommand.ExecuteScalar();
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
                        insertOrderCommand.Parameters.AddWithValue("@CartId", orderr.CartId);
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
