using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel;

namespace API.Controllers.Admin
{
    [Route("AdminDashboard")]
    [ApiController]
    public class AdminDashboardController : Controller
    {
        private string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        [HttpPost]
        public IEnumerable<Product> GetAllProducts([FromBody] CategoryIdModel categoryid)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int categoryId = categoryid.CategoryId;
                string sqlQuery = $"SELECT * FROM Product WHERE CategoryId = {categoryId}";
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

            return products;
        }
    }
}
