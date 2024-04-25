using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API.Controllers
{
    [Route("AdminDashboard")]
    [ApiController]
    public class AdminDashboardController : Controller
    {
        private string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM Product";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Product product = new Product
                    {
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        ProductName = Convert.ToString(reader["ProductName"]),
                        ProductDesc = Convert.ToString(reader["ProductDesc"]),
                        ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]),
                        ProductPrice = Convert.ToInt32(reader["ProductPrice"])                   
                    };
                    products.Add(product);
                }
                reader.Close();
            }

            return (IActionResult)products;
        }
    }
}
