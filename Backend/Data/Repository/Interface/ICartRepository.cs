using Data.Models.CartModels;


namespace Data.Repository.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartItem>> AddCart(int cartId, IEnumerable<CartItem> cart);
        Task DeleteCartItem(int productId);
        Task<CartItem> UpdateCartItem(int productId, int quantity,int cartId);
        Task<IEnumerable<CartItem>> GetCartItems(int cartId);
        Task DeleteCart(int cartId);
    }
}
