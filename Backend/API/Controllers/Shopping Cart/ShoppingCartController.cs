﻿using Data.Models;
using Data.Models.CartModels;
using Data.Models.DTO;
using Service.Services.Implementation;
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
        public async Task<CartItem> AddItemToCart(int productId, int quantity)
        {
            var user = HttpContext.User as ClaimsPrincipal;         
            var cartIdClaim = user.FindFirst("CartId");
            int cartId = 0;
            if (cartIdClaim != null)
            {

                cartId = int.Parse(cartIdClaim.Value);
            }
            return await _cartService.AddItemToCartAsync(productId, quantity,cartId);  
        }

        [HttpGet("Get/CartItems")]
        public async Task<IEnumerable<CartItem>> GetCartItems()
        {
            var user = HttpContext.User as ClaimsPrincipal;
            var cartIdClaim = user.FindFirst("CartId");
            int cartId = 0;
            if (cartIdClaim != null)
            {

                cartId = int.Parse(cartIdClaim.Value);
            }

            return await _cartService.GetCartItemsAsync(cartId);      
        }


        [HttpPut("Update/CartItem")]
        public async  Task<CartItem> UpdateCartItem(int productId, int quantity)
        {
            var user = HttpContext.User as ClaimsPrincipal;
            var cartIdClaim = user.FindFirst("CartId");
            int cartId = 0;
            if (cartIdClaim != null)
            {

                cartId = int.Parse(cartIdClaim.Value);
            }

            return await _cartService.UpdateCartItemAsync(productId, quantity,cartId); 
        }

        [HttpDelete("Delete/CartItem")]
        public async Task DeleteCartItem(int productId)
        {
            await _cartService.DeleteCartItemAsync(productId);   
        }

        [HttpDelete("Delete/Cart")]
        public async Task DeleteCart()
        {

            var user = HttpContext.User as ClaimsPrincipal;
            var cartIdClaim = user.FindFirst("CartId");
            int cartId = 0;
            if (cartIdClaim != null)
            {

                cartId = int.Parse(cartIdClaim.Value);
            }
            await _cartService.DeleteCartAsync(cartId);
        }
    }
}
