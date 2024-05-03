﻿using API.Models;
using API.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace API.Repository.Implementation
{
    public class CartRepository : ICartRepository
    {
        private readonly IConfiguration _configuration;
        private readonly StackExchange.Redis.IDatabase _database;
        public CartRepository(IConfiguration configuration, IConnectionMultiplexer redis) {
            _configuration = configuration;
            _database = redis.GetDatabase();
        }

        
        public async Task<CartItem> AddCartItem(int productId, int quantity)
        {
            CartItem cartItem = new CartItem();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string insertCartQuery = "INSERT INTO Cart (UserId) VALUES (@UserId);";
                SqlCommand insertCartCommand = new SqlCommand(insertCartQuery, connection);       
                insertCartCommand.Parameters.AddWithValue("@UserId", 1);

                int CartId = await insertCartCommand.ExecuteNonQueryAsync();

               

                // Check if the product ID already exists in the CartItem table
                string checkProductQuery = "SELECT COUNT(*) FROM CartItem WHERE ProductId = @ProductId";
                SqlCommand checkProductCommand = new SqlCommand(checkProductQuery, connection);
                checkProductCommand.Parameters.AddWithValue("@ProductId", productId);

                int existingCount = (int)await checkProductCommand.ExecuteScalarAsync();

                if (existingCount > 0)
                {
                    throw new InvalidOperationException("Product ID already exists in the CartItem table.");
                }

                string insertCartItemQuery = "INSERT INTO CartItem (ProductId, Quantity,CartId) VALUES (@ProductId, @Quantity,@CartId); SELECT SCOPE_IDENTITY();";
                SqlCommand insertCartItemCommand = new SqlCommand(insertCartItemQuery, connection);
                insertCartItemCommand.Parameters.AddWithValue("@ProductId", productId);
                insertCartItemCommand.Parameters.AddWithValue("@Quantity", quantity);
                insertCartItemCommand.Parameters.AddWithValue("@CartId", CartId);

                object result = await insertCartItemCommand.ExecuteScalarAsync();
                int cartItemId = Convert.ToInt32(result);

                

                cartItem.ProductId = productId;
                cartItem.Quantity = quantity;
            }

            return cartItem;
        }


        async Task ICartRepository.DeleteCartItem(int productId)
        {

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();

                /*string sqlCartItemIdQuery = "SELECT 1 FROM CartItem WHERE CartItemId = @CartItemId";
                using (SqlCommand checkCommand = new SqlCommand(sqlCategoryIdQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@CartItemId", CartItemId);
                    object cartItemIdObj = await checkCommand.ExecuteScalarAsync();

                    if (cartItemIdObj == null || cartItemIdObj == DBNull.Value)
                    {
                        throw new InvalidOperationException("CartItem does not exist.");
                    }
                }*/

                string deleteQuery = $"DELETE FROM CartItem WHERE ProductId = {productId}; SELECT SCOPE_IDENTITY();";
                using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@ProductId", productId);
                    await deleteCommand.ExecuteNonQueryAsync();
                }

                // Remove cart item from cache
               /* string cacheKey = $"Category_{categoryId}";
                await _database.KeyDeleteAsync(cacheKey);*/

            }
            
        }

        async Task<IEnumerable<CartItem>> ICartRepository.GetCartItems(int cartId)
        {
            List<CartItem> cartItems = new List<CartItem>();
            string cacheKey = "CartItems";
            string cachedCartItems = await _database.StringGetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedCartItems))
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cachedCartItems);
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
                {
                    await connection.OpenAsync();

                    string sqlQuery = $" SELECT * FROM CartItem WHERE CartId = {cartId};";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    // Read data from the first result set (Cart table)
                    while (await reader.ReadAsync())
                    {
                       
                                CartItem cartItem = new CartItem
                                {
                                    ProductId = Convert.ToInt32(reader["ProductId"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"])
                                };
                                cartItems.Add(cartItem);
                            
                        
                    }
                    reader.Close();
                }

   
                await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(cartItems));
            }

            return cartItems;
        }

        async Task<CartItem> ICartRepository.UpdateCartItem(int productId, int quantity)
        {
            CartItem cartItem = new CartItem();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string sqlQuery = "UPDATE CartItem SET Quantity = @Quantity;";
                SqlCommand updateCommand = new SqlCommand(sqlQuery, connection);
                updateCommand.Parameters.AddWithValue("@Quantity", quantity);
                int newId = Convert.ToInt32(updateCommand.ExecuteScalar());
                cartItem.ProductId = newId;
                cartItem.Quantity = quantity;
            }

            return cartItem;
        }
    }
}