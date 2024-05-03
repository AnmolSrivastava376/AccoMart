using API.Models;
using API.Models.DTO;
using API.Services.Implementation;
using API.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MimeKit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.Controllers.ShoppingCart
{
    [Route("ShoppingCartController")]
    [ApiController]
    public class ShoppingCartController : Controller
    {


        private readonly ICartService _cartService;

        private string connectionString = "Server=tcp:acco-mart.database.windows.net,1433;Initial Catalog=Accomart;Persist Security Info=False;User ID=anmol;Password=kamal.kumar@799;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";



        public ShoppingCartController(ICartService cartService)
        {
            _cartService = cartService;
        }


        [HttpPost("Add/CartItem")]
        public async Task<CartItem> AddItemToCart(int productId, int quantity)
        {
         
            return await _cartService.AddItemToCartAsync(productId, quantity);  
        }

        [HttpGet("Get/CartItems")]
        public async Task<IEnumerable<CartItem>> GetCartItems(int cartId)
        {

            return await _cartService.GetCartItemsAsync(cartId);      
        }





        [HttpPut("Update/CartItem")]
        public async  Task<CartItem> UpdateCartItem(int productId, int quantity)
        {
            return await _cartService.UpdateCartItemAsync(productId, quantity); 
        }

        [HttpDelete("Delete/CartItem")]
        public async Task DeleteCartItem(int productId)
        {
            await _cartService.DeleteCartItemAsync(productId);   
        }
    }
}
