using Data.Models.CartModels;
using API.DTO;
using Service.Services.Implementation;
using Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers.ShoppingCart
{
    [Route("ShoppingCartController")]
    [ApiController]
    public class ShoppingCartController : Controller
    {

        private readonly ICartService _cartService;

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
