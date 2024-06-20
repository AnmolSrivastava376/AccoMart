using Data.Models.Product_Category;
using Data.Models.Product_Category.Category;
using Data.Models.Product_Category.Product;
using Data.Models.ViewModels;
using Data.Models.ViewModels.UpdateProduct;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;


using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Data.Repository.Implementation
{

    public class ProductRepository : IProductRepository
    {
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly string connectionstring = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");

        public ProductRepository( IConnectionMultiplexer redis)
        {

            _database = redis.GetDatabase();
        }
        public async Task<List<Category>> GetAllCategories()
        {
            List<Category> categories;
            
                categories = new List<Category>();
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    string sqlQuery = "SELECT * FROM Category";
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
               
            
            return categories;
        }

        public async Task<List<Category>> GetAllCategoriesAdmin(string userId)
        {
            List<Category> categories;
           
                categories = new List<Category>();
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    string sqlQuery = "SELECT * FROM Category WHERE AdminID = @UserId";
                 
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@UserId", userId);
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
              
            return categories;
        }

        public async Task<List<Category>> GetAllCategoriesAdmin(string userId)
        {
            List<Category> categories;
            string cacheKey = "Categories";
            string cachedCategories = await _database.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedCategories))
            {
                categories = JsonConvert.DeserializeObject<List<Category>>(cachedCategories);
            }
            else
            {
                categories = new List<Category>();
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    string sqlQuery = "SELECT * FROM Category WHERE AdminId = @UserId";
                 
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@UserId", userId);
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
                await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(categories));
            }
            return categories;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                string sqlQuery = $"SELECT * FROM Product";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                await connection.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Product product = new Product
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            ProductName = Convert.ToString(reader["ProductName"]),
                            ProductDesc = Convert.ToString(reader["ProductDesc"]),
                            ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]),
                            ProductPrice = Convert.ToInt32(reader["ProductPrice"]),
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            Stock = Convert.ToInt32(reader["Stock"])
                        };
                        products.Add(product);
                    }
                    reader.Close();
                }
            }
            return products;
        }
        public async Task<List<Product>> GetAllProductsPagewise(int pageNo, int pageSize, string userId)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                int offset = (pageNo - 1) * pageSize;
                string sqlQuery = @"
            SELECT * FROM 
            (SELECT ROW_NUMBER() OVER (ORDER BY ProductId) AS RowNum, * FROM Product WHERE AdminId = @AdminId) AS Temp 
            WHERE RowNum >= @Offset AND RowNum < @Limit";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@Limit", offset + pageSize);
                command.Parameters.AddWithValue("@AdminId", userId);

                await connection.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Product product = new Product
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            ProductName = Convert.ToString(reader["ProductName"]),
                            ProductDesc = Convert.ToString(reader["ProductDesc"]),
                            ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]),
                            ProductPrice = Convert.ToInt32(reader["ProductPrice"]),
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            Stock = Convert.ToInt32(reader["Stock"])
                        };
                        products.Add(product);
                    }
                    reader.Close();
                }
            }
            return products;
        }
        public async Task<List<Product>> GetProductsByPageNo(int id, int pageNo, int pageSize)
        {

            List<Product> products = new List<Product>();
         

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    int offset = (pageNo - 1) * pageSize;

                    string sqlQuery = $"SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ProductId) AS RowNum, * FROM Product WHERE CategoryId = @CategoryId) AS Temp WHERE RowNum >= @Offset AND RowNum < @Limit";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@CategoryId", id);
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@Limit", offset + pageSize);

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Product product = new Product
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductName = Convert.ToString(reader["ProductName"]),
                                ProductDesc = Convert.ToString(reader["ProductDesc"]),
                                ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]),
                                ProductPrice = Convert.ToInt32(reader["ProductPrice"]),
                                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                Stock = Convert.ToInt32(reader["Stock"])
                            };
                            products.Add(product);
                        }
                        reader.Close();
                    }
                }
            
            return products;
        }

        public async Task<List<Product>> GetAllProductsByCategory(int id, string orderBy)
        {
            string order = string.IsNullOrEmpty(orderBy) ? "price_asc" : "price_dsc";
            List<Product> products = new List<Product>();
            string cacheKey = $"ProductByCategory_{id}";
            string cachedProducts = null;

            if ((!string.IsNullOrWhiteSpace(cachedProducts) && cachedProducts.Trim() != "[]"))
            {
                products = JsonConvert.DeserializeObject<List<Product>>(cachedProducts);
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    string sqlQuery = $"SELECT * FROM Product WHERE CategoryId = {id}";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Product product = new Product
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductName = Convert.ToString(reader["ProductName"]),
                                ProductDesc = Convert.ToString(reader["ProductDesc"]),
                                ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]),
                                ProductPrice = Convert.ToInt32(reader["ProductPrice"]),
                                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                Stock = Convert.ToInt32(reader["Stock"])
                            };
                            products.Add(product);
                        }
                        reader.Close();
                    }
                }
                await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(products));
            }
            switch (orderBy)
            {
                case "price_dsc":
                    products = products.OrderByDescending(product => product.ProductPrice).ToList();
                    break;
                default:
                    products = products.OrderBy(product => product.ProductPrice).ToList();
                    break;
            }

            return products;
        }



        public async Task<Category> GetCategoryById(int id,string userId)
        {
            
                Category category = await FetchCategoryFromSQL(id,userId);
             
                return category;
            
        }



        private async Task<Category> FetchCategoryFromSQL(int id, string userId)
        {
            Category category = new Category();
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                string sqlQuery = $"SELECT * FROM Category WHERE CategoryId = {id} AND AdminID = {userId}";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@UserId", userId);
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

        public async Task<Product> GetProductById(int id, string userId)
        {

            string cacheKey = $"Product_{id}";
            string cachedProduct = await _database.StringGetAsync(cacheKey);

            if (cachedProduct != null)
            {
                return JsonConvert.DeserializeObject<Product>(cachedProduct);
            }
            else
            {
                Product product = new Product();

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {

                    await connection.OpenAsync();
                    string sqlQuery = $"SELECT * FROM Product WHERE ProductId = {id} AND AdminID = @userId";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    SqlDataReader reader = await command.ExecuteReaderAsync();


                    while (await reader.ReadAsync())
                    {
                        product.ProductId = Convert.ToInt32(reader["ProductId"]);
                        product.ProductName = Convert.ToString(reader["ProductName"]);
                        product.ProductDesc = Convert.ToString(reader["ProductDesc"]);
                        product.ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]);
                        product.ProductPrice = Convert.ToInt32(reader["ProductPrice"]);
                        product.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                        product.Stock = Convert.ToInt32(reader["Stock"]);
                    }
                    reader.Close();
                }

                await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(product));

                return product;
            }
        }

        public async Task<Category> CreateCategory(string categoryName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string query = $"SELECT CategoryName FROM Category WHERE CategoryName = @CategoryName";
                            SqlCommand checkCommand = new SqlCommand(query, connection, transaction);
                            checkCommand.Parameters.AddWithValue("@CategoryName", categoryName);
                            object existingCategory = await checkCommand.ExecuteScalarAsync();

                            if (existingCategory != null)
                            {
                                return null;
                            }

                            string sqlQuery = "INSERT INTO Category (CategoryName) VALUES (@CategoryName); SELECT SCOPE_IDENTITY()";
                            SqlCommand command = new SqlCommand(sqlQuery, connection, transaction);
                            command.Parameters.AddWithValue("@CategoryName", categoryName);
                            int categoryId = Convert.ToInt32(await command.ExecuteScalarAsync());

                            Category category = new Category
                            {
                                CategoryId = categoryId,
                                CategoryName = categoryName
                            };
 
                            transaction.Commit();

                            return category;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Failed to add category: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add category: {ex.Message}");
            }
        }

        public async Task<Product> CreateProduct(ViewProduct productDto,string userId)
        {
            try
            {
                Product product = new Product();
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string sqlQuery = "INSERT INTO Product (ProductName, ProductDesc, ProductPrice, ProductImageUrl, CategoryId,Stock,AdminId) " +
                                              "VALUES (@ProductName, @ProductDesc, @ProductPrice, @ProductImageUrl, @CategoryId,@Stock,@AdminId); SELECT SCOPE_IDENTITY()";
                            SqlCommand command = new SqlCommand(sqlQuery, connection, transaction);
                            command.Parameters.AddWithValue("@ProductName", productDto.ProductName);
                            command.Parameters.AddWithValue("@ProductDesc", productDto.ProductDesc);
                            command.Parameters.AddWithValue("@ProductPrice", productDto.ProductPrice);
                            command.Parameters.AddWithValue("@ProductImageUrl", productDto.ProductImageUrl);
                            command.Parameters.AddWithValue("@CategoryId", productDto.CategoryId);
                            command.Parameters.AddWithValue("@Stock", productDto.Stock);
                            command.Parameters.AddWithValue("@AdminId", userId);


                            int productId = Convert.ToInt32(await command.ExecuteScalarAsync());

                            product.ProductId = productId;
                            product.ProductName = productDto.ProductName;
                            product.ProductDesc = productDto.ProductDesc;
                            product.ProductImageUrl = productDto.ProductImageUrl;
                            product.ProductPrice = productDto.ProductPrice;
                            product.CategoryId = productDto.CategoryId;
                            product.Stock = productDto.Stock;

                            string cacheKey = $"ProductByCategory_{productDto.CategoryId}";
                            await _database.KeyDeleteAsync(cacheKey);
                            string cacheKey2 = $"Product_{product.ProductId}";
                            await _database.StringSetAsync(cacheKey2, JsonConvert.SerializeObject(product));
                            transaction.Commit();

                            return product;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Failed to add product: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add product: {ex.Message}");
            }
        }

        public async Task<Category> UpdateCategory(int categoryId, string newCategoryName)
        {
            try
            {
                Category category = new Category();

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string sqlQuery = "UPDATE Category SET CategoryName = @NewCategoryName WHERE CategoryId = @CategoryId";
                            SqlCommand updateCommand = new SqlCommand(sqlQuery, connection, transaction);
                            updateCommand.Parameters.AddWithValue("@NewCategoryName", newCategoryName);
                            updateCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                            await updateCommand.ExecuteNonQueryAsync();
                            category.CategoryId = categoryId;
                            category.CategoryName = newCategoryName;

                            string cacheKey = $"Category_{category.CategoryId}";
                            await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(category));
                            string categoriesCacheKey = $"Categories";
                            await _database.KeyDeleteAsync(categoriesCacheKey);
                            transaction.Commit();

                            return category;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Failed to update category: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update category: {ex.Message}");
            }
        }

        public async Task<Product> UpdateProduct(int productId, UpdateProduct productDto)
        {
            try
            {
                Product product = new Product();

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string sqlProductIdQuery = "SELECT ProductId FROM Product WHERE ProductId = @ProductId";
                            SqlCommand productIdCommand = new SqlCommand(sqlProductIdQuery, connection, transaction);
                            productIdCommand.Parameters.AddWithValue("@ProductId", productId);
                            object productIdObj = await productIdCommand.ExecuteScalarAsync();

                            if (productIdObj == null || productIdObj == DBNull.Value)
                            {
                                return null;
                            }

                            string sqlCategoryIdQuery = "SELECT CategoryId FROM Category WHERE CategoryId = @CategoryId";
                            SqlCommand categoryIdCommand = new SqlCommand(sqlCategoryIdQuery, connection, transaction);
                            categoryIdCommand.Parameters.AddWithValue("@CategoryId", productDto.CategoryId);
                            object categoryIdObj = await categoryIdCommand.ExecuteScalarAsync();

                            if (categoryIdObj == null || categoryIdObj == DBNull.Value)
                            {
                                return null;
                            }

                            string sqlQuery = "UPDATE Product SET ProductName = @ProductName, ProductDesc = @ProductDesc, " +
                                              "ProductPrice = @ProductPrice, ProductImageUrl = @ProductImageUrl, " +
                                              "CategoryId = @CategoryId, Stock = @Stock WHERE ProductId = @ProductId";

                            SqlCommand updateCommand = new SqlCommand(sqlQuery, connection, transaction);
                            updateCommand.Parameters.AddWithValue("@ProductName", productDto.ProductName);
                            updateCommand.Parameters.AddWithValue("@ProductDesc", productDto.ProductDesc);
                            updateCommand.Parameters.AddWithValue("@ProductPrice", productDto.ProductPrice);
                            updateCommand.Parameters.AddWithValue("@ProductImageUrl", productDto.ProductImageUrl);
                            updateCommand.Parameters.AddWithValue("@CategoryId", productDto.CategoryId);
                            updateCommand.Parameters.AddWithValue("@Stock", productDto.Stock);
                            updateCommand.Parameters.AddWithValue("@ProductId", productId);

                            await updateCommand.ExecuteNonQueryAsync();

                            product.ProductId = productId;
                            product.ProductName = productDto.ProductName;
                            product.ProductDesc = productDto.ProductDesc;
                            product.ProductImageUrl = productDto.ProductImageUrl;
                            product.ProductPrice = productDto.ProductPrice;
                            product.CategoryId = productDto.CategoryId;
                            product.Stock = productDto.Stock;

                            string cacheKey = $"Product_{product.ProductId}";
                            await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(product));
                            transaction.Commit();

                            return product;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Failed to update product: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update product: {ex.Message}");
            }
        }

        public async Task DeleteCategory(int categoryId)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();
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

                string deleteQuery = "DELETE FROM Category WHERE CategoryId = @CategoryId";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                    await deleteCommand.ExecuteNonQueryAsync();
                }
            }

        }

        public async Task<Category> GetCategoryByName(string name,string userId)
        {
            int categoryId = 0;
           
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    string sqlCategoryIdQuery = $"SELECT CategoryId FROM Category WHERE CategoryName = '{name}' AND AdminID = {userId}";

                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sqlCategoryIdQuery, connection))
                    {
                        object categoryIdObj = await command.ExecuteScalarAsync();

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
            
            
                using (SqlConnection connection = new SqlConnection(connectionstring))
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
               
            return category;
        }

        public async Task DeleteProduct(int productId)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
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
            string cacheKey = $"Product_{productId}";
            await _database.KeyDeleteAsync(cacheKey);
        }

        public async Task<List<Product>> GetProductBySearchName(string prefix = "")
        {
            prefix = string.IsNullOrEmpty(prefix) ? "" : prefix.ToLower();
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM Product WHERE ProductName LIKE '{prefix}%';";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Product product = new Product();
                    product.ProductId = Convert.ToInt32(reader["ProductId"]);
                    product.ProductName = Convert.ToString(reader["ProductName"]);
                    product.ProductDesc = Convert.ToString(reader["ProductDesc"]);
                    product.ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]);
                    product.ProductPrice = Convert.ToDecimal(reader["ProductPrice"]);
                    product.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                    product.Stock = Convert.ToInt32(reader["Stock"]);

                    products.Add(product);
                }
                reader.Close();
            }

            return products;
        }

        public async Task<List<Product>> GetProductBySearchNameAdmin(string userId, string prefix = "")
        {
            prefix = string.IsNullOrEmpty(prefix) ? "" : prefix.ToLower();
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM Product WHERE ProductName LIKE '{prefix}%' AND AdminID = {userId};";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Product product = new Product();
                    product.ProductId = Convert.ToInt32(reader["ProductId"]);
                    product.ProductName = Convert.ToString(reader["ProductName"]);
                    product.ProductDesc = Convert.ToString(reader["ProductDesc"]);
                    product.ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]);
                    product.ProductPrice = Convert.ToDecimal(reader["ProductPrice"]);
                    product.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                    product.Stock = Convert.ToInt32(reader["Stock"]);

                    products.Add(product);
                }
                reader.Close();
            }

            return products;
        }

        public async Task<List<Product>> GetProductsByCategoryName(string name = "")
        {
            name = string.IsNullOrEmpty(name) ? "" : name.ToLower();
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                string sqlQuery = @"
            SELECT p.* 
            FROM Product p 
            INNER JOIN Category c ON p.CategoryId = c.CategoryId 
            WHERE LOWER(c.CategoryName) = @name;
        ";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@name", name);

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Product product = new Product();
                    product.ProductId = Convert.ToInt32(reader["ProductId"]);
                    product.ProductName = Convert.ToString(reader["ProductName"]);
                    product.ProductDesc = Convert.ToString(reader["ProductDesc"]);
                    product.ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]);
                    product.ProductPrice = Convert.ToDecimal(reader["ProductPrice"]);
                    product.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                    product.Stock = Convert.ToInt32(reader["Stock"]);

                    products.Add(product);
                }
                reader.Close();
            }

            return products;
        }
    }
}