using Data.Models.CartModels;

namespace Data.Repository.Interfaces
{
    public interface ICartRepository
    {
        Task<CartItem> AddCartItem(int productId, int quantity);
        Task DeleteCartItem(int productId);
        Task<CartItem> UpdateCartItem(int productId, int quantity);
        Task<IEnumerable<CartItem>> GetCartItems(int cartId);



    }
}
