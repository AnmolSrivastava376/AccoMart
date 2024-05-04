using Data.Models.CartModels;
using Data.Models;

namespace Service.Services.Interface
{
    public interface ICartService
    {
        Task<CartItem> AddItemToCartAsync(int productId, int quantity);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId);
        Task<CartItem> UpdateCartItemAsync(int productId, int quantity);
        Task DeleteCartItemAsync(int productId);
        Task<int> AddToCartAsync();
        Task AddUser(Users user);
    }
}
