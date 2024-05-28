using Data.Models.CartModels;
namespace Service.Services.Interface
{
    public interface ICartService
    {
        Task<IEnumerable<CartItem>> AddToCartAsync(int cartId, IEnumerable<CartItem> cart);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId);
        Task<int> AddToCartAsync();
        Task AddUser(Users user);
    }
}
