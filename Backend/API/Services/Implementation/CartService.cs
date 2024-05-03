using API.Models;
using API.Repository.Interfaces;
using API.Services.Interface;

namespace API.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }
        async Task<CartItem> ICartService.AddItemToCartAsync(int productId, int quantity)
        {
            return await _cartRepository.AddCartItem(productId, quantity); 
        }

        async Task ICartService.DeleteCartItemAsync(int productId)
        {
             await _cartRepository.DeleteCartItem(productId);
        }

        async Task<CartItem> ICartService.UpdateCartItemAsync(int productId, int quantity)
        {
            return await _cartRepository.UpdateCartItem(productId, quantity);
        }

        async Task<IEnumerable<CartItem>> ICartService.GetCartItemsAsync(int cartId)
        {
            return await _cartRepository.GetCartItems(cartId);
        }
    }
}
