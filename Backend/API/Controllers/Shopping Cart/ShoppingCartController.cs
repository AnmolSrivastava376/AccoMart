using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API.Controllers.Shopping_Cart
{
    [Route("ShoppingCartController")]
    [ApiController]
    public class ShoppingCartController : Controller
    {

        private string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [HttpPost]
        public CartItem AddProductToCart(int productId, int quantity)
        {
            Product product = new Product();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = $"SELECT * FROM Product WHERE ProductId = {productId}";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    product.ProductId = Convert.ToInt32(reader["ProductId"]);
                    product.ProductName = Convert.ToString(reader["ProductName"]);
                    product.ProductDesc = Convert.ToString(reader["ProductDesc"]);
                    product.ProductImageUrl = Convert.ToString(reader["ProductImageUrl"]);
                    product.ProductPrice = Convert.ToInt32(reader["ProductPrice"]);
                }
                reader.Close();
            }

            var newItem = new CartItem()
            {
                Product = product,
                Quantity = quantity
            };

            if (Cart.GetCartList().Find(cartItem => cartItem.Product.ProductId == productId) == null)
            {
                Cart.GetCartList().Add(newItem);
            }

            return newItem;
        }

        [HttpGet]
        public List<CartResponse> GetCart()
        {
            List<CartResponse> cart = new List<CartResponse>();
            foreach (var item in Cart.GetCartList())
            {
                cart.Add(new CartResponse() { Name = item.Product.ProductName, Price = item.Product.ProductPrice, Quantity = item.Quantity });
            }
            return cart;
        }

        [HttpPut]
        public CartResponse UpdateItem(int productId, int quantity)
        {
            var item = Cart.GetCartList().Find(cartItem => cartItem.Product.ProductId == productId);

            if (item != null)
            {
                var responseItem = new CartResponse()
                { Quantity = quantity, Price = item.Product.ProductPrice, Name = item.Product.ProductName };
                return responseItem;
            }
            else
            {
                return null;
            }
        }

        [HttpDelete]
        public CartResponse? DeleteItem(int productId)
        {
            var item = Cart.GetCartList().Find(cartItem => cartItem.Product.ProductId == productId);

            if (item != null)
            {
                Cart.GetCartList().Remove(item);
            }
            return null;
        }
    }
}
