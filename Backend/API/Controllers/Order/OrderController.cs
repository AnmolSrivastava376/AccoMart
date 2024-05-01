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



        [HttpPost]
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
/*
        [HttpPost]
        public IActionResult PlaceOrderByCart(CartOrderDto order)
        {



            return Ok();
        }*/

    }
}
