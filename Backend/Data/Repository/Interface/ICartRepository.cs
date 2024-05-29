using Data.Models.CartModels;


namespace Data.Repository.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartItem>> AddCart(int cartId, IEnumerable<CartItem> cart);
        Task<IEnumerable<CartItem>> GetCartItems(int cartId);
        Task DeleteCart(int cartId);
    }
}
