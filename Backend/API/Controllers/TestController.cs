using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace API.Controllers
{
    [Route("TestController")]
    [ApiController]
    public class TestController : Controller
    {
        private string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        // GET: api/Admin

        [HttpGet]
        public IEnumerable<Admin> GetAdmins()
        {
            List<Admin> admins = new List<Admin>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM Admin";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Admin admin = new Admin
                    {
                        AdminId = Convert.ToInt32(reader["AdminId"]),
                        AdminEmail = Convert.ToString(reader["AdminEmail"]),
                        AdminPassword = Convert.ToString(reader["AdminPassword"]),

                        // Populate other properties accordingly
                    };
                    admins.Add(admin);
                }
                reader.Close();
            }

            return admins;
        }


        [HttpPost]
        public IActionResult PostAdmin([FromBody]Admin admin)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "INSERT INTO Admin (AdminEmail, AdminPassword) VALUES (@Value1, @Value2)";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@Value1", admin.AdminEmail); // Replace Value1 with the actual property of Admin
                command.Parameters.AddWithValue("@Value2", admin.AdminPassword);
                // Set parameter values
                connection.Open();
                command.ExecuteNonQuery();
            }

            return Ok();
        }



    }
}
