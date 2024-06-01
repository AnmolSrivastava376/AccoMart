using Data.Models.Delivery;
using Data.Models.ViewModels;
using Data.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Data.Repository.Implementation
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly IConfiguration _configuration;

        public DeliveryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task AddDeliveryService(CreateDeliveryService deliveryService)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string sqlQuery = @"INSERT INTO DeliveryService (ImageUrl, ServiceName, Price, DeliveryDays) 
                                        VALUES (@ImageUrl, @ServiceName, @Price, @DeliveryDays)";

                            using (SqlCommand command = new SqlCommand(sqlQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@ImageUrl", deliveryService.ImageUrl);
                                command.Parameters.AddWithValue("@ServiceName", deliveryService.ServiceName);
                                command.Parameters.AddWithValue("@Price", deliveryService.Price);
                                command.Parameters.AddWithValue("@DeliveryDays", deliveryService.DeliveryDays);

                                await command.ExecuteNonQueryAsync();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Failed to add delivery service: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add delivery service: {ex.Message}");
            }
        }


        public async Task DeleteDeliveryService(int id)
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
                            throw new KeyNotFoundException("Delivery service not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete delivery service: {ex.Message}");
            }
        }

        public async Task UpdateDeliveryService(int id, CreateDeliveryService deliveryService)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string sqlQuery = @"UPDATE DeliveryService 
                                        SET ImageUrl = @ImageUrl, ServiceName = @ServiceName, Price = @Price, DeliveryDays = @DeliveryDays 
                                        WHERE DServiceId = @Id";

                            using (SqlCommand command = new SqlCommand(sqlQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Id", id);
                                command.Parameters.AddWithValue("@ImageUrl", deliveryService.ImageUrl);
                                command.Parameters.AddWithValue("@ServiceName", deliveryService.ServiceName);
                                command.Parameters.AddWithValue("@Price", deliveryService.Price);
                                command.Parameters.AddWithValue("@DeliveryDays", deliveryService.DeliveryDays);

                                int rowsAffected = await command.ExecuteNonQueryAsync();

                                if (rowsAffected == 0)
                                {
                                    throw new KeyNotFoundException("Delivery service not found.");
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Failed to update delivery service: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update delivery service: {ex.Message}");
            }
        }


        public async Task<List<DeliveryService>> GetAllDeliveryServices()
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

                return deliveryServices;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve delivery services: {ex.Message}");
            }
        }

        public async Task<int> GetDeliveryDays(int deliveryId)
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

                return days;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve delivery days: {ex.Message}");
            }
        }
    }
}
