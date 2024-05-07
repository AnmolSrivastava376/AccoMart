using Data.Models.CartModels;

using Data.Repository.Interfaces;
using Data.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Newtonsoft.Json;
using Data.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Data.Models.DTO;
using System.Reflection.Metadata.Ecma335;



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


        public async Task<CartItem> AddCartItem(int productId, int quantity, int cardId)
        {
            CartItem cartItem = new CartItem();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();

                // Check if the product ID already exists in the CartItem table
                string checkProductQuery = "SELECT COUNT(*) FROM CartItem WHERE ProductId = @ProductId";
                SqlCommand checkProductCommand = new SqlCommand(checkProductQuery, connection);
                checkProductCommand.Parameters.AddWithValue("@ProductId", productId);

                int existingCount = (int)await checkProductCommand.ExecuteScalarAsync();

                if (existingCount > 0)
                {
                    string updateQuantityQuery = "UPDATE CartItem SET Quantity = Quantity + @Quantity WHERE ProductId = @ProductId";
                    SqlCommand updateQuantityCommand = new SqlCommand(updateQuantityQuery, connection);
                    updateQuantityCommand.Parameters.AddWithValue("@ProductId", productId);
                    updateQuantityCommand.Parameters.AddWithValue("@Quantity", quantity);

                    await updateQuantityCommand.ExecuteNonQueryAsync();

                    // Retrieve the updated cart item after incrementing quantity
                    string getCartItemQuery = "SELECT ProductId, Quantity FROM CartItem WHERE ProductId = @ProductId";
                    SqlCommand getCartItemCommand = new SqlCommand(getCartItemQuery, connection);
                    getCartItemCommand.Parameters.AddWithValue("@ProductId", productId);

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
                    insertCartItemCommand.Parameters.AddWithValue("@CartId", cardId);

                    object result = await insertCartItemCommand.ExecuteScalarAsync();
                    int cartItemId = Convert.ToInt32(result);

                    cartItem.ProductId = productId;
                    cartItem.Quantity = quantity;
                }

            }

            return cartItem;
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

        async Task ICartRepository.GenerateInvoice(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string checkProductQuery = "INSERT INTO Invoice (OrderId) VALUES(@OrderId);";
                SqlCommand checkProductCommand = new SqlCommand(checkProductQuery, connection);
                checkProductCommand.Parameters.AddWithValue("@OrderId", orderId);
                await checkProductCommand.ExecuteScalarAsync();


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

        async Task<GetInvoiceDto> ICartRepository.GetInvoice(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string sqlQuery = $" SELECT * FROM Order WHERE OrderId = {orderId};";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    int? ProductId = reader["ProductId"] as int?;
                    int? CartId = reader["CartId"] as int?;

                    if (ProductId.HasValue)
                    {
                        int actualProductId = ProductId.Value;
                        return await GetInvoiceByProductId(orderId, actualProductId); //----------> optimize this- remove second param
                    }

                    else
                    {
                        //int cartId = CartId.Value;
                        return await GetInvoiceByCart(orderId);
                    }
                }
            }
            return null;
        }
    

       async private Task<GetInvoiceDto> GetInvoiceByCart(int orderId)
       {
            GetInvoiceDto invoiceDto = new GetInvoiceDto();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string sqlQuery = $" SELECT CartId FROM Order WHERE OrderId = @OrderId;";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                SqlDataReader orderReader = await command.ExecuteReaderAsync();
                int cartId = 0;
                while (await orderReader.ReadAsync())
                {
                    cartId = Convert.ToInt32(orderReader["CartId"]);
                }

                SqlDataReader cartItemReader = await command.ExecuteReaderAsync();
                List<InvoiceProductDto> products = new List<InvoiceProductDto>();
                while (await cartItemReader.ReadAsync())
                {

                    string sqlQuery1 = $" SELECT * FROM CartItem WHERE CartId = @CartId;";
                    SqlCommand command1 = new SqlCommand(sqlQuery1, connection);
                    command1.Parameters.AddWithValue("@CartId", cartId);
                    int productId = 0;

                    SqlDataReader productReader = await command.ExecuteReaderAsync();
                    while (await productReader.NextResultAsync())
                    {
                        InvoiceProductDto product = new InvoiceProductDto();
                        productId = Convert.ToInt32(productReader["ProductId"]);
                        string sqlQuery2 = $" SELECT * FROM Product WHERE ProductId = @ProductId;";
                        SqlCommand command2 = new SqlCommand(sqlQuery2, connection);
                        command2.Parameters.AddWithValue("@ProductId", productId);
                        product.ProductName = Convert.ToString(productReader["ProductName"]);
                        product.ProductDesc = Convert.ToString(productReader["ProductDesc"]);
                        product.ProductPrice = (decimal)Convert.ToDouble(productReader["ProductPrice"]);
                        product.Quantity = Convert.ToInt32(cartItemReader["Quantity"]);
                        products.Add(product);
                        
                    }                  
                }

                invoiceDto.products = products;


                //Fetching Order Details
                invoiceDto.OrderDate = Convert.ToDateTime(orderReader["OrderDate"]);
                invoiceDto.OrderTime = Convert.ToDateTime(orderReader["OrderTime"]);
                invoiceDto.OrderAmount = (float)Convert.ToDouble(orderReader["OrderAmount"]);

                    //Fetching User Details
                    string userId = Convert.ToString(orderReader["UserId"]);
                    string sqlQuery3 = $" SELECT * FROM AspNetUsers WHERE Id = @UserId;";
                    SqlCommand command3 = new SqlCommand(sqlQuery3, connection);
                    command3.Parameters.AddWithValue("@UserId", userId);

                SqlDataReader userReader = await command3.ExecuteReaderAsync();
                while (await userReader.NextResultAsync())
                    {
                        invoiceDto.UserName = Convert.ToString(userReader["UserName"]);
                        invoiceDto.UserEmail = Convert.ToString(userReader["UserEmail"]);
                    }


                    //Fetching Address                
                    string sqlQuery4 = $" SELECT * FROM Addresses WHERE UserId = @UserId;";
                    SqlCommand command4 = new SqlCommand(sqlQuery4, connection);
                    command4.Parameters.AddWithValue("@UserId", userId);
                    AddressModel addressModel = new AddressModel();
                SqlDataReader addressReader = await command4.ExecuteReaderAsync();
                while (await addressReader.NextResultAsync())
                    {
                        addressModel.ZipCode = Convert.ToString(addressReader["ZipCode"]);
                        addressModel.PhoneNumber = Convert.ToString(addressReader["PhoneNumber"]);
                        addressModel.City = Convert.ToString(addressReader["City"]);
                        addressModel.State = Convert.ToString(addressReader["State"]);
                        addressModel.Street = Convert.ToString(addressReader["Street"]);
                    }
                    invoiceDto.Address = addressModel;
                


            }
            return invoiceDto;
       }
       async private Task<GetInvoiceDto> GetInvoiceByProductId(int orderId, int productId)
        {
            GetInvoiceDto invoiceDto = new GetInvoiceDto();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string sqlQuery = $" SELECT * FROM Order WHERE OrderId = @OrderId;";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);


                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    //Fecthing Product Details
                    string sqlQuery1 = $" SELECT * FROM Product WHERE ProductId = @ProductId;";
                    SqlCommand command1 = new SqlCommand(sqlQuery1, connection);
                    command1.Parameters.AddWithValue("@ProductId", productId);
                    List<InvoiceProductDto> products = new List<InvoiceProductDto>();
                    InvoiceProductDto product = new InvoiceProductDto();
                    while (await reader.NextResultAsync())
                    {
                        product.ProductName = Convert.ToString(reader["ProductName"]);
                        product.ProductDesc = Convert.ToString(reader["ProductDesc"]);
                        product.ProductPrice = (decimal)Convert.ToInt32(reader["ProductPrice"]);
                    }
                    products.Add(product);
                    invoiceDto.products = products;

                    //Fetching Order Details
                    invoiceDto.OrderDate = Convert.ToDateTime(reader["OrderDate"]);
                    invoiceDto.OrderTime = Convert.ToDateTime(reader["OrderTime"]);
                    invoiceDto.OrderAmount = (float)Convert.ToDouble(reader["OrderAmount"]);

                    //Fetching User Details
                    string userId = Convert.ToString(reader["UserId"]);
                    string sqlQuery2 = $" SELECT * FROM AspNetUsers WHERE Id = @UserId;";
                    SqlCommand command2 = new SqlCommand(sqlQuery2, connection);
                    command2.Parameters.AddWithValue("@UserId", userId);                  
                    while (await reader.NextResultAsync())
                    {
                        invoiceDto.UserName = Convert.ToString(reader["UserName"]);
                        invoiceDto.UserEmail = Convert.ToString(reader["UserEmail"]);                        
                    }


                    //Fetching Address                
                    string sqlQuery3 = $" SELECT * FROM Addresses WHERE UserId = @UserId;";
                    SqlCommand command3 = new SqlCommand(sqlQuery3, connection);
                    command2.Parameters.AddWithValue("@UserId", userId);
                    AddressModel addressModel = new AddressModel();

                    while (await reader.NextResultAsync())
                    {
                        addressModel.ZipCode = Convert.ToString(reader["ZipCode"]);
                        addressModel.PhoneNumber = Convert.ToString(reader["PhoneNumber"]);
                        addressModel.City = Convert.ToString(reader["City"]);
                        addressModel.State = Convert.ToString(reader["State"]);
                        addressModel.Street = Convert.ToString(reader["Street"]);
                    }
                    invoiceDto.Address = addressModel;
                }
            }
            return invoiceDto;
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
