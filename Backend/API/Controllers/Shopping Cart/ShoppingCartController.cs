using Data.Models.CartModels;
using Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


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
        public async Task<IEnumerable<CartItem>> AddCart(int cartId, IEnumerable<CartItem> cart)
        {
            return await _cartService.AddToCartAsync(cartId, cart);  
        }

        [HttpGet("Get/CartItems")]
        public async Task<IEnumerable<CartItem>> GetCartItems(int cartId)
        {
       
            return await _cartService.GetCartItemsAsync(cartId);      
        }

    }
}
