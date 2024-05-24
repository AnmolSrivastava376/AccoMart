using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Data.Models;
using Data.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Service.Models;

namespace API.Controllers.DeliveryServices
{
    [ApiController]
    [Route("DeliveryServiceController")]
    public class DeliveryServiceController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DeliveryServiceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("AddDeliveryService")]
        public async Task<IActionResult> AddDeliveryService(CreateDeliveryServiceDto deliveryService)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();

                    string sqlQuery = @"INSERT INTO DeliveryService (ImageUrl, ServiceName, Price, DeliveryDays) 
                                        VALUES (@ImageUrl, @ServiceName, @Price, @DeliveryDays)";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ImageUrl", deliveryService.ImageUrl);
                        command.Parameters.AddWithValue("@ServiceName", deliveryService.ServiceName);
                        command.Parameters.AddWithValue("@Price", deliveryService.Price);
                        command.Parameters.AddWithValue("@DeliveryDays", deliveryService.DeliveryDays);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new ApiResponse<string> { IsSuccess = true, Message = "Delivery service added successfully.", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"An error occurred while adding the delivery service: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpDelete("DeleteDeliveryService/{id}")]
        public async Task<IActionResult> DeleteDeliveryService(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();

                    string sqlQuery = @"DELETE FROM DeliveryService WHERE DServiceId = @DServiceId";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@DServiceId", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Delivery service not found.", StatusCode = 404 });
                        }
                    }
                }

                return Ok(new ApiResponse<string> { IsSuccess = true, Message = "Delivery service deleted successfully.", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"An error occurred while deleting the delivery service: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpPut("UpdateDeliveryService/{id}")]
        public async Task<IActionResult> UpdateDeliveryService(int id, CreateDeliveryServiceDto deliveryService)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();

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

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Delivery service not found.", StatusCode = 404 });
                        }
                    }
                }

                return Ok(new ApiResponse<string> { IsSuccess = true, Message = "Delivery service updated successfully.", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"An error occurred while updating the delivery service: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("GetAllDeliveryServices")]
        public async Task<IActionResult> GetAllDeliveryServices()
        {
            try
            {
                List<DeliveryService> deliveryServices = new List<DeliveryService>();

                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();

                    string sqlQuery = "SELECT DServiceId, ImageUrl, ServiceName, Price, DeliveryDays FROM DeliveryService";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
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

                return Ok(new ApiResponse<List<DeliveryService>> { IsSuccess = true, Message = "Delivery services retrieved successfully.", StatusCode = 200, Response = deliveryServices });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"An error occurred while retrieving delivery services: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("GetDeliveryDays/{deliveryId}")]
        public async Task<IActionResult> GetDeliveryDays(int deliveryId)
        {
            try
            {
                int days = 0;

                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();

                    string sqlQuery = "SELECT DeliveryDays FROM DeliveryService WHERE DServiceId = @DServiceId";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@DServiceId", deliveryId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                days = Convert.ToInt32(reader["DeliveryDays"]);
                            }
                        }
                    }
                }

                return Ok(new ApiResponse<int> { IsSuccess = true, Message = "Delivery days retrieved successfully.", StatusCode = 200, Response = days });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"An error occurred while retrieving delivery days: {ex.Message}", StatusCode = 500 });
            }
        }
    }
}
