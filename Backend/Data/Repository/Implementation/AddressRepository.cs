using Microsoft.Extensions.Configuration;
using Data.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Data.Models.Address;
namespace Data.Repository.Implementation
{
    public class AddressRepository : IAddressRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private SqlConnection @object;

        public AddressRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"];

        }

        public async Task<int> AddAddressAsync(AddressModel address, string userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        using (var command = new SqlCommand("INSERT INTO Addresses (Street, City, PhoneNumber, States, ZipCode, UserId) VALUES (@Street, @City, @PhoneNumber, @State, @ZipCode, @UserId); SELECT SCOPE_IDENTITY()", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Street", address.Street);
                            command.Parameters.AddWithValue("@City", address.City);
                            command.Parameters.AddWithValue("@PhoneNumber", address.PhoneNumber);
                            command.Parameters.AddWithValue("@State", address.State);
                            command.Parameters.AddWithValue("@ZipCode", address.ZipCode);
                            command.Parameters.AddWithValue("@UserId", userId);

                            int addressId = Convert.ToInt32(await command.ExecuteScalarAsync());
             
                            transaction.Commit();

                            return addressId;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Failed to add address: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add address: {ex.Message}", ex);
            }
        }


        public async Task<AddressModel> GetAddressByIdAsync(int addressId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT * FROM Addresses WHERE AddressId = @AddressId", connection))
                    {
                        command.Parameters.AddWithValue("@AddressId", addressId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new AddressModel
                                {
                                    Street = Convert.ToString(reader["Street"]),
                                    City = Convert.ToString(reader["City"]),
                                    PhoneNumber = Convert.ToString(reader["PhoneNumber"]),
                                    State = Convert.ToString(reader["States"]),
                                    ZipCode = Convert.ToString(reader["ZipCode"]),
                                };
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve address: {ex.Message}", ex);
            }
        }


        public async Task<List<AddressModel>> GetAddressesByUserIdAsync(string userId)
        {
            List<AddressModel> addresses = new List<AddressModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT * FROM Addresses WHERE UserId = @userId", connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                AddressModel address = new AddressModel
                                {
                                    AddressId = Convert.ToInt32(reader["AddressId"]),
                                    Street = Convert.ToString(reader["Street"]),
                                    City = Convert.ToString(reader["City"]),
                                    PhoneNumber = Convert.ToString(reader["PhoneNumber"]),
                                    State = Convert.ToString(reader["States"]),
                                    ZipCode = Convert.ToString(reader["ZipCode"]),
                                };
                                addresses.Add(address);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return addresses;
        }

        public async Task<bool> UpdateAddressAsync(int id, AddressModel address)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var command = new SqlCommand("UPDATE Addresses SET Street = @Street, City = @City, PhoneNumber = @PhoneNumber, States = @State, ZipCode = @ZipCode WHERE AddressId = @AddressId", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Street", address.Street);
                                command.Parameters.AddWithValue("@City", address.City);
                                command.Parameters.AddWithValue("@PhoneNumber", address.PhoneNumber);
                                command.Parameters.AddWithValue("@State", address.State);
                                command.Parameters.AddWithValue("@ZipCode", address.ZipCode);
                                command.Parameters.AddWithValue("@AddressId", id);

                                int rowsAffected = await command.ExecuteNonQueryAsync();
                                if (rowsAffected == 0)
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                                transaction.Commit();
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Failed to update address: {ex.Message}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update address: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("DELETE FROM Addresses WHERE AddressId = @AddressId", connection))
                    {
                        command.Parameters.AddWithValue("@AddressId", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while deleting address with ID {id}.", ex);
            }
        }
    }
}



