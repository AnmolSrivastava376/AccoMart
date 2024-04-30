using API.Models;
using API.Models.DTO;
using API.Repository.Interfaces;
using Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.Repository.Implementation
{

    public class ProductRepository : IProductRepository
    {
        //private string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        /*private readonly ApplicationDbContext dbContext;*/
        private readonly IConfiguration _configuration;
        public ProductRepository(IConfiguration configuration)
        {

            _configuration = configuration;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                string sqlQuery = $"SELECT * FROM Category";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
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

        public async Task<IEnumerable<Product>> GetAllProducts(int id)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                string sqlQuery = $"SELECT * FROM Product WHERE CategoryId = {id}";
                SqlCommand command = new SqlCommand(sqlQuery, connection);

                await connection.OpenAsync(); // Open connection asynchronously

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync()) // Read asynchronously
                    {
                        Product product = new Product
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            ProductName = Convert.ToString(reader["ProductName"]),
                            ProductDesc = Convert.ToString(reader["ProductDesc"]),
                            ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]),
                            ProductPrice = Convert.ToInt32(reader["ProductPrice"]),
                            CategoryId = Convert.ToInt32(reader["CategoryId"])
                        };
                        products.Add(product);
                    }
                }
            }

            return products;
        }

        async Task<Category> IProductRepository.GetCategoryById(int id)
        {
            Category category = new Category();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                string sqlQuery = $"SELECT * FROM Category WHERE CategoryId = {id}";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    category.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                    category.CategoryName = Convert.ToString(reader["CategoryName"]);

                }
                reader.Close();
            }

            return category;
        }

        async Task<Product> IProductRepository.GetProductById(int id)
        {
            Product product = new Product();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                string sqlQuery = $"SELECT * FROM Product WHERE ProductId = {id}";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                _ = connection.OpenAsync(); //--------------------------------------------------- cause of error
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
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

        public async Task<Category> CreateCategory(string categoryName)
        {
            Category category = new Category();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                string query = $"SELECT CategoryName FROM Category WHERE CategoryName = {categoryName};";
                if(query != null)
                {
                    return null;
                }
                string sqlQuery = $"INSERT INTO Category (CategoryName) VALUES (@Value1); SELECT SCOPE_IDENTITY()";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@Value1", categoryName);

                await connection.OpenAsync();
                int categoryId = Convert.ToInt32(await command.ExecuteScalarAsync());

                category.CategoryId = categoryId;
                category.CategoryName = categoryName;
            }
            return category;
        }


        public async Task<Product> CreateProduct(ProductDto productDto, int categoryId)
        {
            Product product = new Product();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string sqlQuery = "INSERT INTO Product (ProductName, ProductDesc, ProductPrice, ProductImageUrl, CategoryId) " +
                                  "VALUES (@ProductName, @ProductDesc, @ProductPrice, @ProductImageUrl, @CategoryId); SELECT SCOPE_IDENTITY()";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@ProductName", productDto.ProductName);
                command.Parameters.AddWithValue("@ProductDesc", productDto.ProductDesc);
                command.Parameters.AddWithValue("@ProductPrice", productDto.ProductPrice);
                command.Parameters.AddWithValue("@ProductImageUrl", productDto.ProductImageUrl);
                command.Parameters.AddWithValue("@CategoryId", categoryId);

                int productId = Convert.ToInt32(await command.ExecuteScalarAsync());

                product.ProductId = productId;
                product.ProductName = productDto.ProductName;
                product.ProductDesc = productDto.ProductDesc;
                product.ProductImageUrl = productDto.ProductImageUrl;
                product.ProductPrice = productDto.ProductPrice;
            }

            return product;
        }




        async Task<Category> IProductRepository.UpdateCategory(string categoryName, string NewCategoryName)
        {
            Category category = new Category();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                string sqlCategoryIdQuery = $"SELECT CategoryId FROM Category WHERE CategoryName = '{categoryName}'";
                int categoryId = 0;

                   await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sqlCategoryIdQuery, connection))
                    {
                        object categoryIdObj = command.ExecuteScalar();

                        if (categoryIdObj != null && categoryIdObj != DBNull.Value)
                        {
                            categoryId = Convert.ToInt32(categoryIdObj);
                        }
                        
                    }

                    string sqlQuery = $"UPDATE Category SET CategoryName = '{NewCategoryName}' WHERE CategoryId = '{categoryId}';";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@CategoryName", NewCategoryName);
                  
                    command.ExecuteNonQuery();
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        category.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                        category.CategoryName = Convert.ToString(reader["CategoryName"]);

                    }
                    reader.Close();
                    
                }
                
            }
            return category;
        }

        async Task<Product> IProductRepository.UpdateProduct(int productId, UpdateProductDto productDto)
        {
            Product product = new Product();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string sqlProductIdQuery = $"SELECT ProductId FROM Product WHERE ProductId = {productId}";
                string sqlCategoryIdQuery = $"SELECT CategoryId FROM Category WHERE CategoryId = {productDto.CategoryId}";
                int ProductId = 0;
                int CategoryId = 0;

                using (SqlCommand command = new SqlCommand(sqlProductIdQuery, connection))
                {
                    object productIdObj = command.ExecuteScalar();

                    if (productIdObj != null && productIdObj != DBNull.Value)
                    {
                        ProductId = Convert.ToInt32(productIdObj);
                    }

                }
                using (SqlCommand command = new SqlCommand(sqlCategoryIdQuery, connection))
                {
                    object categoryIdObj = command.ExecuteScalar();

                    if (categoryIdObj != null && categoryIdObj != DBNull.Value)
                    {
                        CategoryId = Convert.ToInt32(categoryIdObj);
                    }

                }

                string sqlQuery = "UPDATE Product SET ProductName = @ProductName, ProductDesc = @ProductDesc, " +
                                  "ProductPrice = @ProductPrice, ProductImageUrl = @ProductImageUrl, " +
                                  "CategoryId = @CategoryId WHERE ProductId = @ProductId";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@ProductName", productDto.ProductName);
                    command.Parameters.AddWithValue("@ProductDesc", productDto.ProductDesc);
                    command.Parameters.AddWithValue("@ProductPrice", productDto.ProductPrice);
                    command.Parameters.AddWithValue("@ProductImageUrl", productDto.ProductImageUrl);
                    command.Parameters.AddWithValue("@CategoryId", productDto.CategoryId);
                    command.Parameters.AddWithValue("@ProductId", ProductId); // Use the retrieved ProductId                   
                    command.ExecuteNonQuery();
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        product.ProductId = Convert.ToInt32(reader["ProductId"]);
                        product.ProductName = Convert.ToString(reader["ProductName"]);
                        product.ProductDesc = Convert.ToString(reader["ProductDesc"]);
                        product.ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]);
                        product.ProductPrice = Convert.ToInt32(reader["ProductPrice"]);
                    }
                    reader.Close();
                }


               
            }
            return product;
        }

        async Task IProductRepository.DeleteCategory(int CategoryId)
        {
            
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    try
                    {
                        await connection.OpenAsync();
                        string sqlCategoryIdQuery = "SELECT 1 FROM Category WHERE CategoryId = @CategoryId";
                        using (SqlCommand checkCommand = new SqlCommand(sqlCategoryIdQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@CategoryId", CategoryId);
                            object categoryIdObj = await checkCommand.ExecuteScalarAsync();

                            if (categoryIdObj == null || categoryIdObj == DBNull.Value)
                            {
                                throw new InvalidOperationException("Category does not exist.");
                            }
                        }

                        string deleteQuery = "DELETE FROM Category WHERE CategoryId = @CategoryId";
                        using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@CategoryId", CategoryId);
                            await deleteCommand.ExecuteNonQueryAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }        
        }

        Task<Category> IProductRepository.GetCategoryByName(string name)
        {
            
            int categoryId = 0;
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {

                string sqlCategoryIdQuery = $"SELECT CategoryId FROM Category WHERE CategoryName = '{name}'";
                

                  connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlCategoryIdQuery, connection))
                    {
                        object categoryIdObj = command.ExecuteScalar();

                        if (categoryIdObj != null && categoryIdObj != DBNull.Value)
                        {
                            categoryId = Convert.ToInt32(categoryIdObj);
                        }
                        
                    }
                }
            var category = await GetCategoryById_(categoryId);
            return category;
          }


        private async Task<Category> GetCategoryById_(int id)
        {
            Category category = new Category();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                string sqlQuery = $"SELECT * FROM Category WHERE CategoryId = {id}";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    category.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                    category.CategoryName = Convert.ToString(reader["CategoryName"]);

                }
                reader.Close();
            }

            return category;
        }

        async Task IProductRepository.DeleteProduct(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                try
                {
                    await connection.OpenAsync();
                    string sqlProductIdQuery = "SELECT 1 FROM Product WHERE ProductId = @ProductId";
                    using (SqlCommand checkCommand = new SqlCommand(sqlProductIdQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@ProductId", productId);
                        object productIdObj = await checkCommand.ExecuteScalarAsync();

                        if (productIdObj == null || productIdObj == DBNull.Value)
                        {
                            throw new InvalidOperationException("Product does not exist.");
                        }
                    }

                    string deleteQuery = "DELETE FROM Product WHERE ProductId = @ProductId";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@ProductId", productId);
                        await deleteCommand.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

        }
    }
    }



