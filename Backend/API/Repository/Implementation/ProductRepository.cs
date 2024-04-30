using API.Models;
using API.Models.DTO;
using API.Repository.Interfaces;
using Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.Repository.Implementation
{

    public class ProductRepository : IProductRepository
    {
        //private string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        /*private readonly ApplicationDbContext dbContext;*/
        private readonly IConfiguration _configuration;
        private readonly StackExchange.Redis.IDatabase _database;
        public ProductRepository(IConfiguration configuration, IConnectionMultiplexer redis)
        {

            _configuration = configuration;
            _database = redis.GetDatabase();
        }

        /*public async Task<IEnumerable<Category>> GetAllCategories()
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
        }*/


        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            // Check if categories are cached in Redis
            string cacheKey = "Categories";
            string cachedCategories = await _database.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedCategories))
            {
                // If categories are cached, deserialize the JSON string
                categories = JsonConvert.DeserializeObject<List<Category>>(cachedCategories);
            }
            else
            {
                // If categories are not cached, fetch them from the SQL database and cache them
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    string sqlQuery = $"SELECT * FROM Category";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        Category category_ = new Category
                        {
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            CategoryName = Convert.ToString(reader["CategoryName"])
                        };
                        categories.Add(category_);
                    }
                    reader.Close();
                }

                // Cache categories in Redis for future requests
                await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(categories));
            }

            return categories;
        }

        public async Task<IEnumerable<Product>> GetAllProducts(int id)
        {
            List<Product> products = new List<Product>();
            // Check if categories are cached in Redis
            string cacheKey = $"ProductByCategory_{id}";
            string cachedProducts = await _database.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedProducts))
            {
                // If categories are cached, deserialize the JSON string
                products = JsonConvert.DeserializeObject<List<Product>>(cachedProducts);
            }
            else
            {
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
                        reader.Close();
                    }
                }

                await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(products));
            }

            return products;
        }

        public async Task<Category> GetCategoryById(int id)
        {
            string categoryJson = await _database.StringGetAsync($"Category:{id}");

            if (categoryJson != null)
            {
                return JsonConvert.DeserializeObject<Category>(categoryJson);
            }
            else
            {
                // Fetch category from SQL database
                Category category = await FetchCategoryFromSQL(id);

                // Store category in Redis
                await _database.StringSetAsync($"Category:{id}", JsonConvert.SerializeObject(category));

                return category;
            }
        }

        private async Task<Category> FetchCategoryFromSQL(int id)
        {
            Category category = new Category();

            // Connect to SQL database and fetch category
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

        public async Task<Product> GetProductById(int id)
        {
            string cacheKey = $"Product_{id}";
            string cachedProduct = await _database.StringGetAsync(cacheKey);

            if (cachedProduct != null) //------------------------------------------->cause of error
            {
                return JsonConvert.DeserializeObject<Product>(cachedProduct);
            }
            else
            {
                Product product = new Product();

                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();
                    string sqlQuery = $"SELECT * FROM Product WHERE ProductId = {id}";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
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

                // Store product in cache
                await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(product));

                return product;
            }
        }


        public async Task<Category> CreateCategory(string categoryName)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                string query = $"SELECT CategoryName FROM Category WHERE CategoryName = @CategoryName";
                SqlCommand checkCommand = new SqlCommand(query, connection);
                checkCommand.Parameters.AddWithValue("@CategoryName", categoryName);

                await connection.OpenAsync();
                object existingCategory = await checkCommand.ExecuteScalarAsync();

                if (existingCategory != null)
                {
                    // Category already exists, return null or throw an exception
                    return null;
                }

                string sqlQuery = "INSERT INTO Category (CategoryName) VALUES (@CategoryName); SELECT SCOPE_IDENTITY()";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@CategoryName", categoryName);

                int categoryId = Convert.ToInt32(await command.ExecuteScalarAsync());

                Category category = new Category
                {
                    CategoryId = categoryId,
                    CategoryName = categoryName
                };

                // Update cache (if needed)
                string cacheKey = $"Category_{categoryId}";
                string cachedCategory = await _database.StringGetAsync(cacheKey);

                if (cachedCategory != null)
                {
                    await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(category));
                }

                return category;
            }
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

            // Update cache
            string cacheKey = $"Product_{product.ProductId}";
            await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(product));

            return product;
        }

        public async Task<Category> UpdateCategory(string categoryName, string newCategoryName)
        {
            Category category = new Category();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();

                // Get the category ID based on the old category name
                string sqlCategoryIdQuery = "SELECT CategoryId FROM Category WHERE CategoryName = @CategoryName";
                SqlCommand categoryIdCommand = new SqlCommand(sqlCategoryIdQuery, connection);
                categoryIdCommand.Parameters.AddWithValue("@CategoryName", categoryName);
                int categoryId = Convert.ToInt32(await categoryIdCommand.ExecuteScalarAsync());

                // Update the category name in the database
                string sqlQuery = "UPDATE Category SET CategoryName = @NewCategoryName WHERE CategoryId = @CategoryId";
                SqlCommand updateCommand = new SqlCommand(sqlQuery, connection);
                updateCommand.Parameters.AddWithValue("@NewCategoryName", newCategoryName);
                updateCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                await updateCommand.ExecuteNonQueryAsync();

                // Update the category object
                category.CategoryId = categoryId;
                category.CategoryName = newCategoryName;
            }

            // Update cache
            string cacheKey = $"Category_{category.CategoryId}";
            await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(category));

            return category;
        }

        public async Task<Product> UpdateProduct(int productId, UpdateProductDto productDto)
        {
            Product product = new Product();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();

                // Check if the product exists
                string sqlProductIdQuery = "SELECT ProductId FROM Product WHERE ProductId = @ProductId";
                SqlCommand productIdCommand = new SqlCommand(sqlProductIdQuery, connection);
                productIdCommand.Parameters.AddWithValue("@ProductId", productId);
                object productIdObj = await productIdCommand.ExecuteScalarAsync();

                if (productIdObj == null || productIdObj == DBNull.Value)
                {
                    // Product does not exist
                    return null;
                }

                // Check if the category exists
                string sqlCategoryIdQuery = "SELECT CategoryId FROM Category WHERE CategoryId = @CategoryId";
                SqlCommand categoryIdCommand = new SqlCommand(sqlCategoryIdQuery, connection);
                categoryIdCommand.Parameters.AddWithValue("@CategoryId", productDto.CategoryId);
                object categoryIdObj = await categoryIdCommand.ExecuteScalarAsync();

                if (categoryIdObj == null || categoryIdObj == DBNull.Value)
                {
                    // Category does not exist
                    return null;
                }

                // Update the product in the database
                string sqlQuery = "UPDATE Product SET ProductName = @ProductName, ProductDesc = @ProductDesc, " +
                                  "ProductPrice = @ProductPrice, ProductImageUrl = @ProductImageUrl, " +
                                  "CategoryId = @CategoryId WHERE ProductId = @ProductId";
                SqlCommand updateCommand = new SqlCommand(sqlQuery, connection);
                updateCommand.Parameters.AddWithValue("@ProductName", productDto.ProductName);
                updateCommand.Parameters.AddWithValue("@ProductDesc", productDto.ProductDesc);
                updateCommand.Parameters.AddWithValue("@ProductPrice", productDto.ProductPrice);
                updateCommand.Parameters.AddWithValue("@ProductImageUrl", productDto.ProductImageUrl);
                updateCommand.Parameters.AddWithValue("@CategoryId", productDto.CategoryId);
                updateCommand.Parameters.AddWithValue("@ProductId", productId);
                await updateCommand.ExecuteNonQueryAsync();

                // Update the product object
                product.ProductId = productId;
                product.ProductName = productDto.ProductName;
                product.ProductDesc = productDto.ProductDesc;
                product.ProductImageUrl = productDto.ProductImageUrl;
                product.ProductPrice = productDto.ProductPrice;
            }

            // Update cache
            string cacheKey = $"Product_{product.ProductId}";
            await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(product));

            return product;
        }



        public async Task DeleteCategory(int categoryId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();

                // Check if the category exists
                string sqlCategoryIdQuery = "SELECT 1 FROM Category WHERE CategoryId = @CategoryId";
                using (SqlCommand checkCommand = new SqlCommand(sqlCategoryIdQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                    object categoryIdObj = await checkCommand.ExecuteScalarAsync();

                    if (categoryIdObj == null || categoryIdObj == DBNull.Value)
                    {
                        throw new InvalidOperationException("Category does not exist.");
                    }
                }

                // Delete the category from the database
                string deleteQuery = "DELETE FROM Category WHERE CategoryId = @CategoryId";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                    await deleteCommand.ExecuteNonQueryAsync();
                }
            }

            // Remove category from cache
            string cacheKey = $"Category_{categoryId}";
            await _database.KeyDeleteAsync(cacheKey);
        }


        public async Task<Category> GetCategoryByName(string name)
        {
            int categoryId = 0;

            // Check if the category exists in cache
            string cacheKey = $"CategoryByName_{name}";
            string cachedCategoryId = await _database.StringGetAsync(cacheKey);

            if (cachedCategoryId != null)
            {
                categoryId = Convert.ToInt32(cachedCategoryId);
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    string sqlCategoryIdQuery = $"SELECT CategoryId FROM Category WHERE CategoryName = '{name}'";

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sqlCategoryIdQuery, connection))
                    {
                        object categoryIdObj = await command.ExecuteScalarAsync();

                        if (categoryIdObj != null && categoryIdObj != DBNull.Value)
                        {
                            categoryId = Convert.ToInt32(categoryIdObj);

                            // Cache the category ID
                            await _database.StringSetAsync(cacheKey, categoryId.ToString());
                        }
                    }
                }
            }

            // Call GetCategoryById_ asynchronously
            var category = await GetCategoryById_(categoryId);
            return category;
        }

        private async Task<Category> GetCategoryById_(int id)
        {
            Category category = new Category();

            // Check if the category exists in cache
            string cacheKey = $"Category_{id}";
            string cachedCategory = await _database.StringGetAsync(cacheKey);

            if (cachedCategory != null)
            {
                category = JsonConvert.DeserializeObject<Category>(cachedCategory);
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    string sqlQuery = $"SELECT * FROM Category WHERE CategoryId = {id}";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    await connection.OpenAsync();
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        category.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                        category.CategoryName = Convert.ToString(reader["CategoryName"]);
                    }
                    reader.Close();
                }

                // Cache the category object
                await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(category));
            }

            return category;
        }

        public async Task DeleteProduct(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();

                // Check if the product exists
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

                // Delete the product from the database
                string deleteQuery = "DELETE FROM Product WHERE ProductId = @ProductId";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@ProductId", productId);
                    await deleteCommand.ExecuteNonQueryAsync();
                }
            }

            // Remove product from cache
            string cacheKey = $"Product_{productId}";
            await _database.KeyDeleteAsync(cacheKey);
        }

    }
}



