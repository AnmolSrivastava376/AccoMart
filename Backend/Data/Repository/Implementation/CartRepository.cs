using Data.Models.CartModels;
using Data.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Data.Repository.Implementation.Cart

{
    public class CartRepository : ICartRepository
    {
        private readonly IDatabase _database;
        private readonly string connectionstring = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");

        public CartRepository(IConfiguration configuration, IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }


        public async Task<IEnumerable<CartItem>> AddCart(int cartId, IEnumerable<CartItem> cart)
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
                            string deleteCartItemsQuery = "DELETE FROM CartItem WHERE CartId = @CartId";
                            SqlCommand deleteCartItemsCommand = new SqlCommand(deleteCartItemsQuery, connection, transaction);
                            deleteCartItemsCommand.Parameters.AddWithValue("@CartId", cartId);
                            await deleteCartItemsCommand.ExecuteNonQueryAsync();
                            foreach (var item in cart)
                            {
                                int productId = item.ProductId;
                                int quantity = item.Quantity;

                                string insertCartItemQuery = "INSERT INTO CartItem (ProductId, Quantity, CartId) VALUES (@ProductId, @Quantity, @CartId)";
                                SqlCommand insertCartItemCommand = new SqlCommand(insertCartItemQuery, connection, transaction);
                                insertCartItemCommand.Parameters.AddWithValue("@ProductId", productId);
                                insertCartItemCommand.Parameters.AddWithValue("@Quantity", quantity);
                                insertCartItemCommand.Parameters.AddWithValue("@CartId", cartId);
                                await insertCartItemCommand.ExecuteNonQueryAsync();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Failed to add cart items: {ex.Message}", ex);
                        }
                    }
                }

                return cart;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add cart items: {ex.Message}", ex);
            }
        }



        async Task ICartRepository.DeleteCart(int cartId)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
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



        async Task<IEnumerable<CartItem>> ICartRepository.GetCartItems(int cartId)
        {
            List<CartItem> cartItems = new List<CartItem>();

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                string sqlQuery = $" SELECT * FROM CartItem WHERE CartId = {cartId};";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

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

            return cartItems;
        }
    }
}
