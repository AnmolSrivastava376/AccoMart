using API.Models;
using API.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel;

namespace API.Controllers.Admins
{
    [Route("AdminDashboard")]
    [ApiController]
    public class AdminDashboardController : Controller
    {
        private string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        [HttpGet("{id}")]
        public IEnumerable<Product> GetAllProducts([FromRoute] int id)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //int categoryId = categoryid.CategoryId;
                string sqlQuery = $"SELECT * FROM Product WHERE CategoryId = {id}";
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
                        ProductPrice = Convert.ToInt32(reader["ProductPrice"]) ,
                        CategoryId = Convert.ToInt32(reader["CategoryId"])
                    };
                    products.Add(product);
                }
                reader.Close();
            }

            return products;
        }

        [HttpGet("id")]
        public Product GetProductById(int id)
        {
            Product product = new Product();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = $"SELECT * FROM Product WHERE ProductId = {id}";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    product.ProductId = Convert.ToInt32(reader["ProductId"]);
                    product.ProductName = Convert.ToString(reader["ProductName"]);
                    product.ProductDesc = Convert.ToString(reader["ProductDesc"]);
                    product.ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]);
                    product.ProductPrice = Convert.ToInt32(reader["ProductPrice"]);
                }
                reader.Close();
            }

            return product;

        }

        [HttpGet]
        public IEnumerable<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //int categoryId = categoryid.CategoryId;
                string sqlQuery = $"SELECT * FROM Category";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Category category_ = new Category
                    {
                        CategoryId = Convert.ToInt32(reader["CategoryId"]),
                        CategoryName = Convert.ToString(reader["CategoryName"]),

                    };
                    categories.Add(category_);
                }
                reader.Close();
            }

            return categories;
        }

       
       [HttpGet("category/id")]
        public Category GetCategoryById(int id)
        {
            Category category = new Category();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = $"SELECT * FROM Category WHERE CategoryId = {id}";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    category.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                    category.CategoryName = Convert.ToString(reader["CategoryName"]);

                }
                reader.Close();
            }

            return category;
        }


        [HttpPost("category/create")]
         public IActionResult CreateCategory(string categoryName)
         {

            using (SqlConnection connection = new SqlConnection(connectionString))
             {

                 string sqlQuery = $"INSERT INTO Category (CategoryName) VALUES (@Value1)";
                 SqlCommand command = new SqlCommand(sqlQuery, connection);
                 command.Parameters.AddWithValue("@Value1", categoryName); // Replace Value1 with the actual property of Admin               
                 connection.Open();
                 command.ExecuteNonQuery();

             }

             return Ok();
         }

        [HttpPost("product/create")]
        public IActionResult CreateProduct(ProductDto productDto)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string CategoryName = productDto.CategoryName;

                string sqlCategoryIdQuery = $"SELECT CategoryId FROM Category WHERE CategoryName = '{CategoryName}'";
                int categoryId = 0;

                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlCategoryIdQuery, connection))
                    {
                        object categoryIdObj = command.ExecuteScalar();

                        if (categoryIdObj != null && categoryIdObj != DBNull.Value)
                        {
                            categoryId = Convert.ToInt32(categoryIdObj);
                        }
                        else
                        {
                            return NotFound("Category not found");
                        }
                    }

                    string sqlQuery = "INSERT INTO Product (ProductName, ProductDesc, ProductPrice, ProductImageUrl, CategoryId) " +
                                      "VALUES (@ProductName, @ProductDesc, @ProductPrice, @ProductImageUrl, @CategoryId)";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProductName", productDto.ProductName);
                        command.Parameters.AddWithValue("@ProductDesc", productDto.ProductDesc);
                        command.Parameters.AddWithValue("@ProductPrice", productDto.ProductPrice);
                        command.Parameters.AddWithValue("@ProductImageUrl", productDto.ProductImageUrl);
                        command.Parameters.AddWithValue("@CategoryId", categoryId);

                        command.ExecuteNonQuery();
                    }

                    return Ok("Product created successfully");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred while creating the product: {ex.Message}");
                }
            }
        }



        /*[HttpPost]
        public IActionResult PostAdmin([FromBody] Admin admin)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "INSERT INTO Admin (AdminEmail, AdminPassword) VALUES (@Value1, @Value2)";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@Value1", admin.AdminEmail);
                command.Parameters.AddWithValue("@Value2", admin.AdminPassword);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return Ok();
        }*/







    }
}
