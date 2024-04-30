using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API.Controllers.Order
{


    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [HttpPost]
        public IActionResult PlaceOrder(Orders order)
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

                    string sqlQuery = @"INSERT INTO Orders (OrderDate, OrderTime, AddressId, UserId) 
                                        VALUES (@OrderDate, @OrderTime, @AddressId, @UserId);
                                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        command.Parameters.AddWithValue("@OrderTime", order.OrderTime);
                        command.Parameters.AddWithValue("@AddressId", order.AddressId);
                        command.Parameters.AddWithValue("@UserId", order.UserId);

                        int newOrderId = Convert.ToInt32(command.ExecuteScalar());
                        order.OrderId = newOrderId;
                    }
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while placing the order: {ex.Message}");
            }
        }
    }
}
