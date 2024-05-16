using Data.Models.CartModels;
using Data.Models;

namespace Service.Services.Interface
{
    public interface ICartService
    {
        Task<IEnumerable<CartItem>> AddToCartAsync(int cartId, IEnumerable<CartItem> cart);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId);
        Task<CartItem> UpdateCartItemAsync(int productId, int quantity, int cartId);
        Task DeleteCartItemAsync(int productId);
        Task<int> AddToCartAsync();
        Task AddUser(Users user);
       
        Task DeleteCartAsync(int cartId);
    }
}
