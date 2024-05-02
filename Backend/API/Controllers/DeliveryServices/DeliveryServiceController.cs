using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using API.Models;
using System;
using System.Collections.Generic;

namespace API.Controllers.DeliveryServices
{
    [ApiController]
    [Route("DeliveryServiceController")]
    public class DeliveryServiceController : ControllerBase
    {
        private readonly string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [HttpPost("AddDeliveryService")]
        public IActionResult AddDeliveryService(DeliveryService deliveryService)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = @"INSERT INTO DeliveryService (ImageUrl, ServiceName, Price, DeliveryDays) 
                                        VALUES (@ImageUrl, @ServiceName, @Price, @DeliveryDays)";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ImageUrl", deliveryService.ImageUrl);
                        command.Parameters.AddWithValue("@ServiceName", deliveryService.ServiceName);
                        command.Parameters.AddWithValue("@Price", deliveryService.Price);
                        command.Parameters.AddWithValue("@DeliveryDays", deliveryService.DeliveryDays);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Delivery service added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the delivery service: {ex.Message}");
            }
        }

        [HttpDelete("DeleteDeliveryService/{id}")]
        public IActionResult DeleteDeliveryService(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = @"DELETE FROM DeliveryService WHERE DServiceId = @DServiceId";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@DServiceId", id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound("Delivery service not found.");
                        }
                    }
                }

                return Ok("Delivery service deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the delivery service: {ex.Message}");
            }
        }

        [HttpPut("UpdateDeliveryService/{id}")]
        public IActionResult UpdateDeliveryService(int id, DeliveryService deliveryService)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = @"UPDATE DeliveryService 
                                        SET ImageUrl = @ImageUrl, ServiceName = @ServiceName, Price = @Price, DeliveryDays = @DeliveryDays 
                                        WHERE DServiceId = @Id";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@ImageUrl", deliveryService.ImageUrl);
                        command.Parameters.AddWithValue("@ServiceName", deliveryService.ServiceName);
                        command.Parameters.AddWithValue("@Price", deliveryService.Price);
                        command.Parameters.AddWithValue("@DeliveryDays", deliveryService.DeliveryDays);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound("Delivery service not found.");
                        }
                    }
                }

                return Ok("Delivery service updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the delivery service: {ex.Message}");
            }
        }

        [HttpGet("GetAllDeliveryServices")]
        public IActionResult GetAllDeliveryServices()
        {
            try
            {
                List<DeliveryService> deliveryServices = new List<DeliveryService>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = "SELECT DServiceId, ImageUrl, ServiceName, Price, DeliveryDays FROM DeliveryService";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DeliveryService deliveryService = new DeliveryService
                                {
                                    DServiceId = Convert.ToInt32(reader["DServiceId"]),
                                    ImageUrl = reader["ImageUrl"].ToString(),
                                    ServiceName = reader["ServiceName"].ToString(),
                                    Price = (float)Convert.ToDecimal(reader["Price"]),
                                    DeliveryDays = Convert.ToInt32(reader["DeliveryDays"])
                                };

                                deliveryServices.Add(deliveryService);
                            }
                        }
                    }
                }

                return Ok(deliveryServices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving delivery services: {ex.Message}");
            }
        }
    }
}
