using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Data.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
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

        [HttpPost("PostAddress")]
        public async Task<IActionResult> PostAddress(AddressModel address, int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                return Ok("Address added successfully.");
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                                return Ok(address);
                            }
                            else
                            {
                                return NotFound("Address not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateAddress/addressId={id}")]
        public async Task<IActionResult> UpdateAddress(int id, AddressModel address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                            return NotFound("Address not found.");
                        }
                    }
                }
                return Ok("Address updated successfully.");
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                            return NotFound("Address not found.");
                        }
                    }
                }
                return Ok("Address deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
