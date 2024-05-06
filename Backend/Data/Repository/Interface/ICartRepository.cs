using Data.Models.CartModels;

namespace Data.Repository.Interfaces
{
    public interface ICartRepository
    {
        Task<CartItem> AddCartItem(int productId, int quantity, int cardId);
        Task DeleteCartItem(int productId);
        Task<CartItem> UpdateCartItem(int productId, int quantity,int cartId);
        Task<IEnumerable<CartItem>> GetCartItems(int cartId);
        Task GenerateInvoice(int cartId);
        Task GetInvoice(int orderId);
    }
}
