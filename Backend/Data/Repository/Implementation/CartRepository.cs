using Data.Models.CartModels;

using Data.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Newtonsoft.Json;
using Data.Models;

using Data.Models.DTO;

using Microsoft.AspNetCore.Mvc;

using Azure;
using System.Xml.Linq;




namespace Data.Repository.Implementation

{
    public class CartRepository : ICartRepository
    {
        private readonly IConfiguration _configuration;
        private readonly StackExchange.Redis.IDatabase _database;
        public CartRepository(IConfiguration configuration, IConnectionMultiplexer redis) {
            _configuration = configuration;
            _database = redis.GetDatabase();
        }


        public async Task<IEnumerable<CartItem>> AddCart(int cartId, IEnumerable<CartItem> cart)
        {
            IEnumerable<CartItem> cart1 = cart;
            CartItem cartItem = new CartItem();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                foreach (var item in cart)
                {
                    int productId = item.ProductId;
                    int quantity = item.Quantity;

                    // Check if the product ID already exists in the CartItem table
                    string checkProductQuery = "SELECT COUNT(*) FROM CartItem WHERE ProductId = @ProductId and CartId = @CartId";
                    SqlCommand checkProductCommand = new SqlCommand(checkProductQuery, connection);
                    checkProductCommand.Parameters.AddWithValue("@ProductId", productId);
                    checkProductCommand.Parameters.AddWithValue("@CartId", cartId);

                    int existingCount = (int)await checkProductCommand.ExecuteScalarAsync();

                    if (existingCount > 0)
                    {
                        string updateQuantityQuery = "UPDATE CartItem SET Quantity = Quantity + @Quantity WHERE ProductId = @ProductId and CartId = @CartId ";
                        SqlCommand updateQuantityCommand = new SqlCommand(updateQuantityQuery, connection);
                        updateQuantityCommand.Parameters.AddWithValue("@ProductId", productId);
                        updateQuantityCommand.Parameters.AddWithValue("@Quantity", quantity);
                        updateQuantityCommand.Parameters.AddWithValue("@CartId", cartId);

                        await updateQuantityCommand.ExecuteNonQueryAsync();

                        // Retrieve the updated cart item after incrementing quantity
                        string getCartItemQuery = "SELECT ProductId, Quantity FROM CartItem WHERE ProductId = @ProductId and  CartId = @CartId";
                        SqlCommand getCartItemCommand = new SqlCommand(getCartItemQuery, connection);
                        getCartItemCommand.Parameters.AddWithValue("@ProductId", productId);
                        getCartItemCommand.Parameters.AddWithValue("@CartId", cartId);


                        using (SqlDataReader reader = await getCartItemCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                cartItem.ProductId = reader.GetInt32(0);
                                cartItem.Quantity = reader.GetInt32(1);
                            }
                        }
                    }
                    else
                    {
                        string insertCartItemQuery = "INSERT INTO CartItem (ProductId, Quantity, CartId) VALUES (@ProductId, @Quantity, @CartId); SELECT SCOPE_IDENTITY();";
                        SqlCommand insertCartItemCommand = new SqlCommand(insertCartItemQuery, connection);
                        insertCartItemCommand.Parameters.AddWithValue("@ProductId", productId);
                        insertCartItemCommand.Parameters.AddWithValue("@Quantity", quantity);
                        insertCartItemCommand.Parameters.AddWithValue("@CartId", cartId);

                        object result = await insertCartItemCommand.ExecuteScalarAsync();
                        int cartItemId = Convert.ToInt32(result);

                        cartItem.ProductId = productId;
                        cartItem.Quantity = quantity;
                    }
                }

            }

            return cart1;
        }

        async Task ICartRepository.DeleteCart(int cartId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();

                string sqlCartIdQuery = "DELETE FROM CartItem WHERE CartId = @CartId";
                using (SqlCommand checkCommand = new SqlCommand(sqlCartIdQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@CartId", cartId);
                    object cartIdObj = await checkCommand.ExecuteScalarAsync();

                    if (cartIdObj == null || cartIdObj == DBNull.Value)
                    {
                        throw new InvalidOperationException("Cart is already empty.");
                    }
                }               
            }
        }

        async Task ICartRepository.DeleteCartItem(int cartItemId)
        {

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();

                string sqlCartItemIdQuery = "SELECT 1 FROM CartItem WHERE CartItemId = @CartItemId";
                using (SqlCommand checkCommand = new SqlCommand(sqlCartItemIdQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@CartItemId", cartItemId);
                    object cartItemIdObj = await checkCommand.ExecuteScalarAsync();

                    if (cartItemIdObj == null || cartItemIdObj == DBNull.Value)
                    {
                        throw new InvalidOperationException("CartItem does not exist.");
                    }
                }



                // Remove cart item from cache
                /* string cacheKey = $"Category_{categoryId}";
                 await _database.KeyDeleteAsync(cacheKey);*/

            }

        }


        async Task<IEnumerable<CartItem>> ICartRepository.GetCartItems(int cartId)
        {
            List<CartItem> cartItems = new List<CartItem>();
            /*string cacheKey = "CartItems";
            string cachedCartItems = await _database.StringGetAsync(cacheKey);*/
            /*if (!string.IsNullOrEmpty(cachedCartItems))
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cachedCartItems);
            }*/


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



                //await _database.StringSetAsync(cacheKey, JsonConvert.SerializeObject(cartItems));
            }

            return cartItems;
        }


        async Task<CartItem> ICartRepository.UpdateCartItem(int productId, int quantity, int cartId)
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
