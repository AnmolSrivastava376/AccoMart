using Data.Models;
using Data.Models.DTO;
using Data.Models.Statistic_Models;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Diagnostics;
using System.Linq;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Data.Repository.Implementation
{

    public class ProductRepository : IProductRepository
    {
        private readonly IConfiguration _configuration;
        private readonly StackExchange.Redis.IDatabase _database;
        public ProductRepository(IConfiguration configuration, IConnectionMultiplexer redis)
        {

            _configuration = configuration;
            _database = redis.GetDatabase();
        }

        public async Task<List<Category>> GetAllCategories()
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
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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
                await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(categories));
            }
            return categories;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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

        public async Task<List<Product>> GetProductsByPageNoAsync(int id, int pageNo, int pageSize)
        {

            List<Product> products = new List<Product>();
            string cacheKey = $"ProductsPage{id}_{pageNo}_{pageSize}";
            string cachedProducts = await _database.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedProducts))
            {
                products = JsonConvert.DeserializeObject<List<Product>>(cachedProducts);
  
            }

            else
            {
                
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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
                await _database.StringSetAsync($"ProductsPage{id}_{pageNo}_{pageSize}", JsonConvert.SerializeObject(products));
            }
            return products;
        }

        public async Task<List<Product>> GetAllProductsByCategoryAsync(int id, string orderBy)
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
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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



        public async Task<Category> GetCategoryById(int id)
        {
            string categoryJson = await _database.StringGetAsync($"Category:{id}");

            if (categoryJson != null)
            {
                return JsonConvert.DeserializeObject<Category>(categoryJson);
            }
            else
            {
                Category category = await FetchCategoryFromSQL(id);
                await _database.StringSetAsync($"Category:{id}", JsonConvert.SerializeObject(category));

                return category;
            }
        }


        public async Task<Category> GetCategoryByName(int id)
        {
            string categoryJson = await _database.StringGetAsync($"Category:{id}");

            if (categoryJson != null)
            {
                return JsonConvert.DeserializeObject<Category>(categoryJson);
            }
            else
            {
                Category category = await FetchCategoryFromSQL(id);
                await _database.StringSetAsync($"Category:{id}", JsonConvert.SerializeObject(category));

                return category;
            }
        }

        private async Task<Category> FetchCategoryFromSQL(int id)
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

        public async Task<Product> GetProductById(int id)
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
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                string query = $"SELECT CategoryName FROM Category WHERE CategoryName = @CategoryName";
                SqlCommand checkCommand = new SqlCommand(query, connection);
                checkCommand.Parameters.AddWithValue("@CategoryName", categoryName);

                await connection.OpenAsync();
                object existingCategory = await checkCommand.ExecuteScalarAsync();

                if (existingCategory != null)
                {
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
                string cacheKey = $"Categories";
                await _database.KeyDeleteAsync(cacheKey);

                return category;
            }
        }

        public async Task<Product> CreateProduct(ProductDto productDto)
        {
            Product product = new Product();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string sqlQuery = "INSERT INTO Product (ProductName, ProductDesc, ProductPrice, ProductImageUrl, CategoryId,Stock) " +
                                  "VALUES (@ProductName, @ProductDesc, @ProductPrice, @ProductImageUrl, @CategoryId,@Stock); SELECT SCOPE_IDENTITY()";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@ProductName", productDto.ProductName);
                command.Parameters.AddWithValue("@ProductDesc", productDto.ProductDesc);
                command.Parameters.AddWithValue("@ProductPrice", productDto.ProductPrice);
                command.Parameters.AddWithValue("@ProductImageUrl", productDto.ProductImageUrl);
                command.Parameters.AddWithValue("@CategoryId", productDto.CategoryId);
                command.Parameters.AddWithValue("@Stock", productDto.Stock);

                int productId = Convert.ToInt32(await command.ExecuteScalarAsync());

                product.ProductId = productId;
                product.ProductName = productDto.ProductName;
                product.ProductDesc = productDto.ProductDesc;
                product.ProductImageUrl = productDto.ProductImageUrl;
                product.ProductPrice = productDto.ProductPrice;
                product.CategoryId = productDto.CategoryId;
                product.Stock = productDto.Stock;
            }

            string cacheKey = $"ProductByCategory_{productDto.CategoryId}";
            await _database.KeyDeleteAsync(cacheKey);
            string cacheKey2 = $"Product_{product.ProductId}";
            await _database.StringSetAsync(cacheKey2, JsonConvert.SerializeObject(product));

            return product;
        }

        public async Task<Category> UpdateCategory(int categoryId, string newCategoryName)
        {
            Category category = new Category();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string sqlQuery = "UPDATE Category SET CategoryName = @NewCategoryName WHERE CategoryId = @CategoryId";
                SqlCommand updateCommand = new SqlCommand(sqlQuery, connection);
                updateCommand.Parameters.AddWithValue("@NewCategoryName", newCategoryName);
                updateCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                await updateCommand.ExecuteNonQueryAsync();
                category.CategoryId = categoryId;
                category.CategoryName = newCategoryName;
            }
            string cacheKey = $"Category_{category.CategoryId}";
            await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(category));
            string categoriesCacheKey = $"Categories";
            await _database.KeyDeleteAsync(categoriesCacheKey);

            return category;
        }

        public async Task<Product> UpdateProduct(int productId, UpdateProductDto productDto)
        {
            Product product = new Product();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string sqlProductIdQuery = "SELECT ProductId FROM Product WHERE ProductId = @ProductId";
                SqlCommand productIdCommand = new SqlCommand(sqlProductIdQuery, connection);
                productIdCommand.Parameters.AddWithValue("@ProductId", productId);
                object productIdObj = await productIdCommand.ExecuteScalarAsync();

                if (productIdObj == null || productIdObj == DBNull.Value)
                {
                    return null;
                }
                string sqlCategoryIdQuery = "SELECT CategoryId FROM Category WHERE CategoryId = @CategoryId";
                SqlCommand categoryIdCommand = new SqlCommand(sqlCategoryIdQuery, connection);
                categoryIdCommand.Parameters.AddWithValue("@CategoryId", productDto.CategoryId);
                object categoryIdObj = await categoryIdCommand.ExecuteScalarAsync();

                if (categoryIdObj == null || categoryIdObj == DBNull.Value)
                {
                    return null;
                }
                string sqlQuery = "UPDATE Product SET ProductName = @ProductName, ProductDesc = @ProductDesc, " +
                                  "ProductPrice = @ProductPrice, ProductImageUrl = @ProductImageUrl, " +
                                  "CategoryId = @CategoryId,Stock =@Stock  WHERE ProductId = @ProductId";

                SqlCommand updateCommand = new SqlCommand(sqlQuery, connection);
                updateCommand.Parameters.AddWithValue("@ProductName", productDto.ProductName);
                updateCommand.Parameters.AddWithValue("@ProductDesc", productDto.ProductDesc);
                updateCommand.Parameters.AddWithValue("@ProductPrice", productDto.ProductPrice);
                updateCommand.Parameters.AddWithValue("@ProductImageUrl", productDto.ProductImageUrl);
                updateCommand.Parameters.AddWithValue("@CategoryId", productDto.CategoryId);
                updateCommand.Parameters.AddWithValue("@ProductId", productId);
                updateCommand.Parameters.AddWithValue("@Stock", productDto.Stock);

                await updateCommand.ExecuteNonQueryAsync();

                product.ProductId = productId;
                product.ProductName = productDto.ProductName;
                product.ProductDesc = productDto.ProductDesc;
                product.ProductImageUrl = productDto.ProductImageUrl;
                product.ProductPrice = productDto.ProductPrice;
                product.Stock = productDto.Stock;
            }
            string cacheKey = $"Product_{product.ProductId}";
            await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(product));
            return product;
        }



        public async Task DeleteCategory(int categoryId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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
            string cacheKey = $"Category_{categoryId}";
            await _database.KeyDeleteAsync(cacheKey);

            string categoriesCacheKey = $"Categories";
            await _database.KeyDeleteAsync(categoriesCacheKey);
        }


        public async Task<Category> GetCategoryByName(string name)
        {
            int categoryId = 0;
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
                            await _database.StringSetAsync(cacheKey, categoryId.ToString());
                        }
                    }
                }
            }
            var category = await GetCategoryById_(categoryId);
            return category;
        }

        private async Task<Category> GetCategoryById_(int id)
        {
            Category category = new Category();
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
                await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(category));
            }

            return category;
        }

        public async Task DeleteProduct(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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

        public async Task<List<Product>> GetProductsByCategoryName(string name = "")
        {
            name = string.IsNullOrEmpty(name) ? "" : name.ToLower();
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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



