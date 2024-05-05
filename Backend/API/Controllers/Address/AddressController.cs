using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Data.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers.Address
{
    [Route("AddressController")]
    [ApiController]
    public class AddressController : Controller
    {
        private readonly string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [Authorize]
        [HttpPost("PostAddress")]
        public IActionResult PostAddress(AddressModel address)
        {
            var user = HttpContext.User as ClaimsPrincipal;

            var userIdClaim = user.FindFirst("UserId");
            string userId = "0";
            if (userIdClaim != null)
            {
                userId = userIdClaim.Value;
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("INSERT INTO Addresses (Street, City, States, ZipCode, UserId) VALUES (@Street, @City, @State, @ZipCode, @UserId)", connection))
                    {
                        command.Parameters.AddWithValue("@Street", address.Street);
                        command.Parameters.AddWithValue("@City", address.City);
                        command.Parameters.AddWithValue("@State", address.State);
                        command.Parameters.AddWithValue("@ZipCode", address.ZipCode);
                        command.Parameters.AddWithValue("@UserId", address.userId);

                        command.ExecuteNonQuery();
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


        [Authorize]
        [HttpPut("UpdateAddress/{id}")]
        public IActionResult UpdateAddress(int id, AddressModel address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("UPDATE Addresses SET Street = @Street, City = @City, States = @State, ZipCode = @ZipCode WHERE AddressId = @AddressId", connection))
                    {
                        command.Parameters.AddWithValue("@Street", address.Street);
                        command.Parameters.AddWithValue("@City", address.City);
                        command.Parameters.AddWithValue("@State", address.State);
                        command.Parameters.AddWithValue("@ZipCode", address.ZipCode);
                        command.Parameters.AddWithValue("@AddressId", id);

                        int rowsAffected = command.ExecuteNonQuery();

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



        [Authorize]
        [HttpDelete("DeleteAddress/{id}")]
        public IActionResult DeleteAddress(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("DELETE FROM Addresses WHERE AddressId = @AddressId", connection))
                    {
                        command.Parameters.AddWithValue("@AddressId", id);
                        int rowsAffected = command.ExecuteNonQuery();

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
