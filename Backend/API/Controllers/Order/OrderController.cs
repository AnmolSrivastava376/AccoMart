using Data.Models.OrderModels;
using Data.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Service.Services.Interface;
using Stripe.Checkout;
using StackExchange.Redis;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Data.Models.Authentication.User;
using Service.Models;
using Org.BouncyCastle.Crypto;
using Data.Models.CartModels;

namespace API.Controllers.Order
{
    [ApiController]
    [Route("OrderController")]
    public class OrderController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly ICartService _cartService;
        private string _domain;     
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string connectionstring= Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");



        public OrderController(IConfiguration configuration, ICartService cartService, IConnectionMultiplexer redis,IEmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _cartService = cartService;
            _database = redis.GetDatabase();
            _emailService = emailService;
            _domain = configuration["Url:frontendUrl"];
            _userManager = userManager;
        }



        [HttpGet("FetchAllOrders/{userId}")]
        public async Task<IActionResult> FetchAllOrders(string userId)
        {
           
            List <Orders> orders = new List<Orders>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();
                    string getAllOrdersQuery = "SELECT * FROM Orders WHERE UserId = @UserId ORDER BY OrderId DESC";
                    using (SqlCommand command = new SqlCommand(getAllOrdersQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Orders order = new Orders
                                {
                                    OrderId = Convert.ToInt32(reader["OrderId"]),
                                    OrderDate = reader["OrderDate"] != DBNull.Value ? Convert.ToDateTime(reader["OrderDate"]) : DateTime.MinValue,
                                    OrderAmount = reader["OrderAmount"] != DBNull.Value ? Convert.ToInt32(reader["OrderAmount"]) : 0,
                                    UserId = reader["UserId"] != DBNull.Value ? Convert.ToString(reader["UserId"]) : "",
                                    AddressId = reader["AddressId"] != DBNull.Value ? Convert.ToInt32(reader["AddressId"]) : 0,
                                    CartId = reader["CartId"] != DBNull.Value ? Convert.ToInt32(reader["CartId"]) : 0,
                                    DeliveryServiceID = Convert.ToInt32(reader["DeliveryServiceId"]),
                                    isCancelled = reader["IsCancelled"] != DBNull.Value && Convert.ToBoolean(reader["IsCancelled"]),
                                };
                                orders.Add(order);
                            }
                        }
                    }
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("PlaceOrderByCart")]
        public async Task<IActionResult> PlaceOrderByCart(CartOrder cartOrderDto)
        {
            int newOrderId = 0;

            try
            {
                decimal productAmount = 0.00M;
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();
                    string getProductAmountQuery = @"
                        SELECT CAST(SUM(p.ProductPrice * ci.Quantity) AS DECIMAL(18,2)) AS TotalAmount
                        FROM CartItem ci
                        JOIN Product p ON ci.ProductId = p.ProductId
                        WHERE ci.CartId = @CartId";

                    using (var getProductAmountCommand = new SqlCommand(getProductAmountQuery, connection))
                    {
                        getProductAmountCommand.Parameters.AddWithValue("@CartId", cartOrderDto.cartId);
                        object result = await getProductAmountCommand.ExecuteScalarAsync();

                        if (result != DBNull.Value)
                        {
                            productAmount = (decimal)result;
                        }
                    }

                    decimal deliveryPrice;
                    string getDeliveryPriceQuery = @"
                        SELECT Price
                        FROM DeliveryService
                        WHERE DServiceId = @DServiceId;";

                    using (var getDeliveryPriceCommand = new SqlCommand(getDeliveryPriceQuery, connection))
                    {
                        getDeliveryPriceCommand.Parameters.AddWithValue("@DServiceId", cartOrderDto.deliveryId);
                        deliveryPrice = (decimal)await getDeliveryPriceCommand.ExecuteScalarAsync();
                    }

                    decimal totalAmount = productAmount + deliveryPrice;

                    string insertOrderQuery = @"
                        INSERT INTO Orders (UserId, AddressId, CartId, DeliveryServiceID, OrderAmount) 
                        VALUES (@UserId, @AddressId, @CartId, @DeliveryServiceID, @OrderAmount);
                        SELECT SCOPE_IDENTITY();";

                    using (var insertOrderCommand = new SqlCommand(insertOrderQuery, connection))
                    {
                        insertOrderCommand.Parameters.AddWithValue("@UserId", cartOrderDto.userId);
                        insertOrderCommand.Parameters.AddWithValue("@AddressId", cartOrderDto.addressId);
                        insertOrderCommand.Parameters.AddWithValue("@CartId", cartOrderDto.cartId);
                        insertOrderCommand.Parameters.AddWithValue("@DeliveryServiceID", cartOrderDto.deliveryId);
                        insertOrderCommand.Parameters.AddWithValue("@OrderAmount", totalAmount);

                        newOrderId = Convert.ToInt32(await insertOrderCommand.ExecuteScalarAsync());
                    }// Creating order , it will be deleted if the ordered amount is greater than the available amount at the time of checkout

                }

                return await CheckoutByCart(cartOrderDto.userId, cartOrderDto.cartId, newOrderId, cartOrderDto.deliveryId, productAmount);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = ex.Message });
            }
        }

        //[Authorize]
        [HttpPost("Checkout/Cart")]
        public async Task<IActionResult> CheckoutByCart(string userId, int cartId, int orderId, int deliveryId, decimal productAmount)
        {
              _domain = "https://agreeable-coast-00adf000f.5.azurestaticapps.net/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = _domain + "home/yourorders",
                CancelUrl = _domain + "home/cart",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "sdfgh@gmail.com",
            };
            StripeModel url = new StripeModel();
            decimal totalAmount;


            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();
                string getCartItemQuery = @"SELECT * FROM CartItem WHERE CartId = @CartId";

                using (var getCartItemCommand = new SqlCommand(getCartItemQuery, connection))
                {
                    getCartItemCommand.Parameters.AddWithValue("@CartId", cartId);

                    using (SqlDataReader cartItemReader = await getCartItemCommand.ExecuteReaderAsync())
                    {
                        List<Tuple<int, int>> CartItems = new List<Tuple<int, int>>();

                        while (await cartItemReader.ReadAsync())
                        {
                            int productId = Convert.ToInt32(cartItemReader["ProductId"]);
                            int productQuantity = Convert.ToInt32(cartItemReader["Quantity"]);

                            CartItems.Add(Tuple.Create(productId, productQuantity));
                        }

                        await cartItemReader.CloseAsync();

                        foreach (Tuple<int, int> CartItem in CartItems)
                        {
                            if (!IsQuantityAvailable(CartItem.Item1, CartItem.Item2))
                            {
                                await DeleteOrder(orderId); // deleting the created product as not enough quanity of products 
                                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "Product out of stock" });
                            }
                        }


                        foreach (Tuple<int, int> CartItem in CartItems)
                        {
                            string insertOrderHistoryQuery = "INSERT INTO OrderHistory (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)";
                            using (var insertHistoryCommand = new SqlCommand(insertOrderHistoryQuery, connection))
                            {
                                insertHistoryCommand.Parameters.AddWithValue("@OrderId", orderId);
                                insertHistoryCommand.Parameters.AddWithValue("@ProductId", CartItem.Item1);
                                insertHistoryCommand.Parameters.AddWithValue("@Quantity", CartItem.Item2);
                                await insertHistoryCommand.ExecuteNonQueryAsync();
                            }
                            UpdateStock(CartItem.Item1, CartItem.Item2);
                        }

                        _cartService.DeleteCartAsync(cartId);
                        foreach (Tuple<int, int> CartItem in CartItems)
                        {
                            string getProductQuery = @"SELECT * FROM Product WHERE ProductId = @ProductId";

                            using (var getProductCommand = new SqlCommand(getProductQuery, connection))
                            {
                                getProductCommand.Parameters.AddWithValue("@ProductId", CartItem.Item1);

                                using (SqlDataReader productReader = await getProductCommand.ExecuteReaderAsync())
                                {
                                    while (await productReader.ReadAsync())
                                    {
                                        string name = Convert.ToString(productReader["ProductName"]);
                                        decimal productPrice = (decimal)productReader["ProductPrice"];

                                        var sessionListItem = new SessionLineItemOptions
                                        {
                                            PriceData = new SessionLineItemPriceDataOptions
                                            {
                                                UnitAmount = (long)(productPrice * 100),
                                                Currency = "inr",
                                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                                {
                                                    Name = name
                                                }
                                            },
                                            Quantity = CartItem.Item2,
                                        };
                                        options.LineItems.Add(sessionListItem);
                                    }
                                }
                            }
                        }

                        decimal deliveryPrice = 0.00M;
                        string deliveryQuery = @"SELECT * FROM DeliveryService WHERE DServiceId = @DServiceId";

                        using (var getDeliveryCommand = new SqlCommand(deliveryQuery, connection))
                        {
                            getDeliveryCommand.Parameters.AddWithValue("@DServiceId", deliveryId);

                            using (SqlDataReader deliveryReader = await getDeliveryCommand.ExecuteReaderAsync())
                            {
                                while (await deliveryReader.ReadAsync())
                                {
                                    deliveryPrice = (decimal)deliveryReader["Price"];
                                    string name = Convert.ToString(deliveryReader["ServiceName"]);
                                    var sessionListItem1 = new SessionLineItemOptions
                                    {
                                        PriceData = new SessionLineItemPriceDataOptions
                                        {
                                            UnitAmount = (long)(deliveryPrice * 100),
                                            Currency = "inr",
                                            ProductData = new SessionLineItemPriceDataProductDataOptions
                                            {
                                                Name = name
                                            }
                                        },
                                        Quantity = (long)1

                                    };
                                    options.LineItems.Add(sessionListItem1);
                                }
                            }
                        }

                        decimal discount = (productAmount * 5) / 100;

                        var sessionListItem2 = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(discount * 100),
                                Currency = "inr",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Discount"
                                }
                            },
                            Quantity = (long)1

                        };
                        options.LineItems.Add(sessionListItem2);

                         totalAmount = deliveryPrice + productAmount - discount;


                        var sessionListItem3 = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(totalAmount * 18),
                                Currency = "inr",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Tax"
                                }
                            },
                            Quantity = (long)1

                        };
                        options.LineItems.Add(sessionListItem3);

                    }
                }
            }

            var service = new SessionService();
            Session session = service.Create(options);
            HttpContext.Session.SetString("Session", session.Id);
            Response.Headers.Add("Location", session.Url);
            url.StripeUrl = session.Url;

            if(url==null)
            {
                await DeleteOrder(orderId);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "Internal server error" });

            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                SendMail(user,totalAmount,orderId);
            }

            return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Checkout successful", StripeModel = url });

        }

        [HttpPost("PlaceOrderByProduct")]
        public async Task<IActionResult> PlaceOrder(ProductOrder productOrderDto)
        {

            try
            {
                int orderId = 0;
                decimal productPrice = 0.00M;
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    decimal deliveryPrice;
                    string getDeliveryPriceQuery = @"
                        SELECT Price
                        FROM DeliveryService
                        WHERE DServiceId = @DServiceId;";

                    using (var getDeliveryPriceCommand = new SqlCommand(getDeliveryPriceQuery, connection))
                    {
                        getDeliveryPriceCommand.Parameters.AddWithValue("@DServiceId", productOrderDto.DeliveryId);
                        deliveryPrice = (decimal)await getDeliveryPriceCommand.ExecuteScalarAsync();
                    }

                    string fetchPriceQuery = "SELECT ProductPrice FROM Product WHERE ProductId = @ProductId";

                    using (SqlCommand fetchPriceCommand = new SqlCommand(fetchPriceQuery, connection))
                    {
                        fetchPriceCommand.Parameters.AddWithValue("@ProductId", productOrderDto.ProductId);
                        productPrice = (decimal)await fetchPriceCommand.ExecuteScalarAsync();
                        decimal orderAmount = productPrice + deliveryPrice;

                        string sqlQuery = @"INSERT INTO Orders (AddressId, UserId, ProductId, DeliveryServiceID, OrderAmount)
                                            VALUES (@AddressId, @UserId, @ProductId, @DeliveryServiceID, @OrderAmount);
                                            SELECT SCOPE_IDENTITY();";

                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@AddressId", productOrderDto.AddressId);
                            command.Parameters.AddWithValue("@UserId", productOrderDto.UserId);
                            command.Parameters.AddWithValue("@ProductId", productOrderDto.ProductId);
                            command.Parameters.AddWithValue("@DeliveryServiceID", productOrderDto.DeliveryId);
                            command.Parameters.AddWithValue("@OrderAmount", orderAmount);
                            orderId = Convert.ToInt32(await command.ExecuteScalarAsync());
                        }
                        // this order will be deleted at the time of checkout if ordered quanity of product is greater than available quantity
                    }
                }

                return await Checkout(productOrderDto.ProductId, productOrderDto.DeliveryId, productPrice, orderId, productOrderDto.quantity,productOrderDto.UserId);


            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "Internal server error" });

            }
        }

        [HttpGet("GetCartItemsByOrderId/{orderId}")]
        public async Task<IActionResult> FetchHistory(int orderId)
        {
            try
            {
                List<OrderedItem> orderedItems = new List<OrderedItem>();
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();
                    string sqlQuery = "SELECT * FROM OrderHistory WHERE OrderId = @OrderId";
                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        OrderedItem item = new OrderedItem
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                        };
                        orderedItems.Add(item);
                    }
                }

                return Ok(orderedItems);
            }
            catch
            {
                return null;
            }
        }


        [HttpPost("Checkout/Product")]
        public async Task<IActionResult> Checkout(int productId, int deliveryId, decimal totalProductPrice, int orderId, int quantity, string userId)
        {
            _domain = _configuration["Url:frontendUrl"];
            decimal totalAmount;
            if (!IsQuantityAvailable(productId, quantity))
            {

                await DeleteOrder(orderId);    // deleting the created order as not enough quantity is available
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "Out of stock" });

            }

            var options = new SessionCreateOptions
            {
                SuccessUrl = _domain + "home/yourorders",
                CancelUrl = _domain + "home/buy-product/:productId",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "sdfgh@gmail.com",

            };

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                string getProductQuery = @"SELECT * FROM Product WHERE ProductId = @ProductId";

                using (var getProductCommand = new SqlCommand(getProductQuery, connection))
                {
                    getProductCommand.Parameters.AddWithValue("@ProductId", productId);
                    await getProductCommand.ExecuteNonQueryAsync();


                    using (SqlDataReader cartItemReader = await getProductCommand.ExecuteReaderAsync())
                    {
                        while (await cartItemReader.ReadAsync())
                        {
                            string name = Convert.ToString(cartItemReader["ProductName"]);
                            string newName = name;
                            decimal productPrice = (decimal)cartItemReader["ProductPrice"];

                            var sessionListItem = new SessionLineItemOptions
                            {
                                PriceData = new SessionLineItemPriceDataOptions
                                {
                                    UnitAmount = (long)(productPrice * 100),
                                    Currency = "inr",
                                    ProductData = new SessionLineItemPriceDataProductDataOptions
                                    {
                                        Name = name
                                    }
                                },
                                Quantity = (long)quantity
                            };

                            options.LineItems.Add(sessionListItem);
                        }

                    }

                    decimal deliveryPrice = 0.00M;
                    string deliveryQuery = @"SELECT * FROM DeliveryService WHERE DServiceId = @DServiceId";

                    using (var getDeliveryCommand = new SqlCommand(deliveryQuery, connection))
                    {
                        getDeliveryCommand.Parameters.AddWithValue("@DServiceId", deliveryId);

                        using (SqlDataReader deliveryReader = await getDeliveryCommand.ExecuteReaderAsync())
                        {
                            while (await deliveryReader.ReadAsync())
                            {
                                deliveryPrice = (decimal)deliveryReader["Price"];
                                string name = Convert.ToString(deliveryReader["ServiceName"]);
                                var sessionListItem1 = new SessionLineItemOptions
                                {
                                    PriceData = new SessionLineItemPriceDataOptions
                                    {
                                        UnitAmount = (long)(deliveryPrice * 100),
                                        Currency = "inr",
                                        ProductData = new SessionLineItemPriceDataProductDataOptions
                                        {
                                            Name = name
                                        }
                                    },
                                    Quantity = (long)1

                                };
                                options.LineItems.Add(sessionListItem1);
                            }
                        }
                    }

                    decimal discount = (totalProductPrice * 5) / 100;
                    var sessionListItem2 = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(discount * 100),
                            Currency = "inr",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Discount"
                            }
                        },
                        Quantity = (long)1

                    };
                    options.LineItems.Add(sessionListItem2);

                   totalAmount = deliveryPrice + totalProductPrice - discount;


                    var sessionListItem3 = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(totalAmount * 18),
                            Currency = "inr",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Tax"
                            }
                        },
                        Quantity = (long)1

                    };
                    options.LineItems.Add(sessionListItem3);

                }

            }

            var service = new SessionService();
            Session session = service.Create(options);
            HttpContext.Session.SetString("Session", session.Id);
            Response.Headers.Add("Location", session.Url);
            StripeModel url = new StripeModel();
            url.StripeUrl = session.Url;

            if (url != null)
            {
                await UpdateStock(productId, quantity);

                string insertOrderHistoryQuery = "INSERT INTO OrderHistory (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)";

                var connection = new SqlConnection(connectionstring);
                await connection.OpenAsync();

                using (var insertHistoryCommand = new SqlCommand(insertOrderHistoryQuery, connection))
                {
                    insertHistoryCommand.Parameters.AddWithValue("@OrderId", orderId);
                    insertHistoryCommand.Parameters.AddWithValue("@ProductId", productId);
                    insertHistoryCommand.Parameters.AddWithValue("@Quantity", quantity);
                    await insertHistoryCommand.ExecuteNonQueryAsync();
                };

                var user = await _userManager.FindByIdAsync(userId);
                if (user != null) 
                {
                    SendMail(user,totalAmount,orderId);
                }

            }
            else
            {

                await DeleteOrder(orderId);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "Internal server error" });

            }

            return Ok(new { StatusCode = HttpStatusCode.OK, Message = "Checkout successful", StripeModel = url });

        }
   

        [HttpPost("Product/Stock/productId={productId}")]
        public async Task<int> GetStockAvailable(int productId)
        {
            return await StockAvailable(productId);
        }

        [HttpPost("Order/Cancel/{orderId}")]
        public async Task<IActionResult>CancelOrder(int orderId, [FromBody] List<OrderItem> orderItems)
        {
            try
            {
                var isCancelled = await isCancelledOrder(orderId);
                if (isCancelled)
                {
                    return BadRequest(new { message = "Order already cancelled" });
                }


                using (var connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();
                    string cancelOrderQuery = "Update Orders set isCancelled= 'true' where orderId =@orderId";
                    using (var cancelOrderCommand = new SqlCommand(cancelOrderQuery, connection))
                    {
                        cancelOrderCommand.Parameters.AddWithValue("@orderId", orderId);
                        await cancelOrderCommand.ExecuteNonQueryAsync();
                    }

                    
                    foreach(OrderItem orderItem in orderItems)
                    {
                        await UpdateStock(orderItem.Product.ProductId, -1 * orderItem.Quantity);   // this will add the ordered quantity again in the inventory
                    }

                }
                return Ok(new { message = "Order Cancelled Successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Error cancelling order: {ex.Message}" });
            }
        }

        private async Task DeleteOrder(int orderId)
        {
            using (var connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();
                string deleteStockQuery = "Delete from Orders where OrderID = @orderId";
                using (var deleteStockCommand = new SqlCommand(deleteStockQuery, connection))
                {
                    deleteStockCommand.Parameters.AddWithValue("@orderId", orderId);
                    await deleteStockCommand.ExecuteNonQueryAsync();
                }
            }
        }
        private async Task UpdateStock(int productId, int purchasedQuantity)
        {
            using (var connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();

                string updateStockQuery = @"UPDATE Product SET Stock = Stock - @PurchasedQuantity WHERE ProductId = @ProductId";

                using (var updateStockCommand = new SqlCommand(updateStockQuery, connection))
                {
                    updateStockCommand.Parameters.AddWithValue("@ProductId", productId);
                    updateStockCommand.Parameters.AddWithValue("@PurchasedQuantity", purchasedQuantity);
                    await updateStockCommand.ExecuteNonQueryAsync();
                }
            }

            string cacheKey = $"Product_{productId}";
            string cachedProduct = await _database.StringGetAsync(cacheKey);
            if (cachedProduct != null)
            {
                await _database.KeyDeleteAsync(cacheKey);

            }
        }

        private bool IsQuantityAvailable(int productId, int requestedQuantity)
        {
            using (var connection = new SqlConnection(connectionstring))
            {
                connection.Open();

                string getStockQuery = @"SELECT Stock FROM Product WHERE ProductId = @ProductId";

                using (var getStockCommand = new SqlCommand(getStockQuery, connection))
                {
                    getStockCommand.Parameters.AddWithValue("@ProductId", productId);
                    int currentStock = (int)getStockCommand.ExecuteScalar();

                    if (requestedQuantity > currentStock)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        private async Task<bool> isCancelledOrder(int orderId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    string isCancelledQuery = @"Select IsCancelled from Orders where OrderId = @OrderId";

                    using (var isCancelledCommand = new SqlCommand(isCancelledQuery, connection))
                    {
                        isCancelledCommand.Parameters.AddWithValue("@OrderId", orderId);
                        var result = await isCancelledCommand.ExecuteScalarAsync();

                        if (result != null && result != DBNull.Value)
                        {
                            return (bool)result;
                        }
                        else
                        {
                            return false; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking if order is cancelled: {ex.Message}");

            }
        }
        private async Task<int> StockAvailable(int productId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionstring))
                {
                    await connection.OpenAsync();

                    string getStockQuery = @"SELECT Stock FROM Product WHERE ProductId = @ProductId";

                    using (var getStockCommand = new SqlCommand(getStockQuery, connection))
                    {
                        getStockCommand.Parameters.AddWithValue("@ProductId", productId);
                        object result = await getStockCommand.ExecuteScalarAsync();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching stock for product ID {productId}: {ex.Message}");
                return 0;
            }
        }
        private string GetProductName(int productId)
        {
            using (var connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                string getProductQuery = "SELECT ProductName FROM Product WHERE Id = @ProductId";
                using (var getProductCommand = new SqlCommand(getProductQuery, connection))
                {
                    getProductCommand.Parameters.AddWithValue("@ProductId", productId);
                    return getProductCommand.ExecuteScalar()?.ToString() ?? "Unknown Product";
                }
            }
        }
        
        private void  SendMail(ApplicationUser user,decimal totalAmount,int orderId)
        {
            string messageBody = $"Dear {user.UserName},\n\n";
            messageBody += $"Thank you for placing an order with Accomart.\n\n";
            messageBody += $"Your order with ID {orderId} has been successfully placed with a total amount of ₹{totalAmount:N2}.\n\n";
            messageBody += $"Best regards,\nAccomart Team";

            var message = new Message(new string[] { user.Email }, "Order Successfully Placed", messageBody);
            _emailService.SendEmail(message);
        }

    }
}