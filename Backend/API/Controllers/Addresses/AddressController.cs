using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Service.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers.Addresses
{
    [Route("AddressController")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AddressController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("PostAddress/{userId}")]
        public async Task<IActionResult> PostAddress(AddressModel address, string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> { IsSuccess = false, Message = "Invalid input data.", StatusCode = 400 });
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("INSERT INTO Addresses (Street, City, PhoneNumber, States, ZipCode, UserId) VALUES (@Street, @City, @PhoneNumber, @State, @ZipCode, @UserId)", connection))
                    {
                        command.Parameters.AddWithValue("@Street", address.Street);
                        command.Parameters.AddWithValue("@City", address.City);
                        command.Parameters.AddWithValue("@PhoneNumber", address.PhoneNumber);
                        command.Parameters.AddWithValue("@State", address.State);
                        command.Parameters.AddWithValue("@ZipCode", address.ZipCode);
                        command.Parameters.AddWithValue("@UserId", userId);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new ApiResponse<string> { IsSuccess = true, Message = "Address added successfully.", StatusCode = 200 });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"Internal server error: {ex.Message}", StatusCode = 500 });

            }
        }

        [HttpGet("GetAddress/addressId={addressId}")]
        public async Task<IActionResult> GetAddress(int addressId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT * FROM Addresses WHERE AddressId = @AddressId", connection))
                    {
                        command.Parameters.AddWithValue("@AddressId", addressId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                AddressModel address = new AddressModel
                                {
                                    Street = Convert.ToString(reader["Street"]),
                                    City = Convert.ToString(reader["City"]),
                                    PhoneNumber = Convert.ToString(reader["PhoneNumber"]),
                                    State = Convert.ToString(reader["States"]),
                                    ZipCode = Convert.ToString(reader["ZipCode"]),
                                };
                                return Ok(new ApiResponse<AddressModel> { IsSuccess = true, Response = address, StatusCode = 200 });
                            }
                            else
                            {
                                return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Address not found.", StatusCode = 404 });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("GetAddress/{userId}")]
        public async Task<IActionResult> GetAddressByUserId(string userId)
        {
            try
            {
                List<AddressModel> addresses = new List<AddressModel>();
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT * FROM Addresses WHERE UserId = @userId", connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
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
                            if (addresses.Count > 0)
                            {
                                return Ok(new ApiResponse<List<AddressModel>> { IsSuccess = true, Response = addresses, StatusCode = 200 });
                            }
                            else
                            {
                                return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Addresses not found.", StatusCode = 404 });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"Internal server error: {e.Message}", StatusCode = 500 });
            }
        }

        [HttpPut("UpdateAddress/addressId={id}")]
        public async Task<IActionResult> UpdateAddress(int id, AddressModel address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> { IsSuccess = false, Message = "Invalid input data.", StatusCode = 400 });
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("UPDATE Addresses SET Street = @Street, City = @City, PhoneNumber = @PhoneNumber,States = @State, ZipCode = @ZipCode WHERE AddressId = @AddressId", connection))
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
                            return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Address not found.", StatusCode = 404 });
                        }
                    }
                }
                return Ok(new ApiResponse<string> { IsSuccess = true, Message = "Address updated successfully.", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpDelete("DeleteAddress/addressId={id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("DELETE FROM Addresses WHERE AddressId = @AddressId", connection))
                    {
                        command.Parameters.AddWithValue("@AddressId", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Address not found.", StatusCode = 404 });
                        }
                    }
                }
                return Ok(new ApiResponse<string> { IsSuccess = true, Message = "Address deleted successfully.", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }
    }
}
