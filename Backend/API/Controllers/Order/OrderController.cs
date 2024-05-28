using Data.Models;
using Data.Models.CartModels;
using Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Service.Services.Interface;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.Controllers.Order
{
    [ApiController]
    [Route("OrderController")]
    public class OrderController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly ICartService _cartService;
        private readonly string _domain = "http://localhost:4200/";

        public OrderController(IConfiguration configuration, ICartService cartService)
        {
            _configuration = configuration;
            _cartService = cartService;
            _connectionString = _configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"];
        }
        [HttpGet("FetchAllOrders/{userId}")]
        public async Task<IActionResult> FetchAllOrders(string userId)
        {
            List<Orders> orders = new List<Orders>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string getAllOrdersQuery = "SELECT * FROM Orders WHERE UserId = @UserId";
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
        public async Task<Data.Models.DTO.Stripe> PlaceOrderByCart(CartOrder cartOrderDto)
        {
            int newOrderId = 0;
           
            try
            {
                decimal productAmount = 0.00M;
                using (SqlConnection connection = new SqlConnection(_connectionString))
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
                        insertOrderCommand.Parameters.AddWithValue("@CartId",  cartOrderDto.cartId);
                        insertOrderCommand.Parameters.AddWithValue("@DeliveryServiceID", cartOrderDto.deliveryId);
                        insertOrderCommand.Parameters.AddWithValue("@OrderAmount", totalAmount);

                        newOrderId = Convert.ToInt32(await insertOrderCommand.ExecuteScalarAsync());
                    }
                    
                }

                return await CheckoutByCart(cartOrderDto.userId, cartOrderDto.cartId,newOrderId,cartOrderDto.deliveryId,productAmount);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //[Authorize]
        [HttpPost("Checkout/Cart")]
        public async Task<Data.Models.DTO.Stripe> CheckoutByCart(string userId, int cartId,int orderId,int deliveryId,decimal productAmount)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = _domain + "home/yourorders",
                CancelUrl = _domain + "home/cart",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "sdfgh@gmail.com",              
            };

            using (var connection = new SqlConnection(_connectionString))
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
                            string insertOrderHistoryQuery = "INSERT INTO OrderHistory (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)";

                            using (var insertHistoryCommand = new SqlCommand(insertOrderHistoryQuery, connection))
                            {
                                insertHistoryCommand.Parameters.AddWithValue("@OrderId", orderId);
                                insertHistoryCommand.Parameters.AddWithValue("@ProductId", CartItem.Item1);
                                insertHistoryCommand.Parameters.AddWithValue("@Quantity", CartItem.Item2);
                                await insertHistoryCommand.ExecuteNonQueryAsync();

                            }

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

                        decimal discount = (productAmount * 5)/100;

                        var sessionListItem2 = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(discount*100),
                                Currency = "inr",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Discount"
                                }
                            },
                            Quantity = (long)1

                        };
                        options.LineItems.Add(sessionListItem2);

                        decimal totalAmount = deliveryPrice + productAmount - discount;


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
            Data.Models.DTO.Stripe url = new Data.Models.DTO.Stripe();
            url.StripeUrl = session.Url;
            return url;
        }

        //[Authorize]
        [HttpPost("PlaceOrderByProduct")]
        public async Task<Data.Models.DTO.Stripe> PlaceOrder(ProductOrder productOrderDto)
        {
          
            try
            {
                int orderId = 0;
                decimal productPrice = 0.00M;
                using (var connection = new SqlConnection(_connectionString))
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

                    }
                }




                return await Checkout(productOrderDto.ProductId, productOrderDto.DeliveryId,productPrice,orderId, productOrderDto.quantity);

         
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("GetCartItemsByOrderId/{orderId}")]
        public async Task<IActionResult> FetchHistory(int orderId)
        {
            try
            {
                List<OrderedItem> orderedItems = new List<OrderedItem>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
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
        public async Task<Data.Models.DTO.Stripe> Checkout(int productId,int deliveryId,decimal totalProductPrice,int orderId,int quantity)
        {

            var options = new SessionCreateOptions
            {
                SuccessUrl = _domain + "home/yourorders",
                CancelUrl = _domain + "home/buy-product/:productId",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "sdfgh@gmail.com",

            };

            using (var connection = new SqlConnection(_connectionString))
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

                    decimal totalAmount = deliveryPrice + totalProductPrice - discount;


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
            Data.Models.DTO.Stripe url = new Data.Models.DTO.Stripe();
            url.StripeUrl = session.Url;

            if(url!= null)
            {
                string insertOrderHistoryQuery = "INSERT INTO OrderHistory (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)";

                var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using (var insertHistoryCommand = new SqlCommand(insertOrderHistoryQuery, connection))
                {
                    insertHistoryCommand.Parameters.AddWithValue("@OrderId", orderId);
                    insertHistoryCommand.Parameters.AddWithValue("@ProductId", productId);
                    insertHistoryCommand.Parameters.AddWithValue("@Quantity", quantity);
                    await insertHistoryCommand.ExecuteNonQueryAsync();

                };
            }
            return url;

        }


        private string GetProductName(int productId)
        {
            using (var connection = new SqlConnection(_connectionString))
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
    }
}
