using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class OrderResponse
    {
       // [Key]
       // public int OrderId { get; set; } 
       //public int UserId { get; set; }
        public required UserAddress Shipping {  get; set; }
        public decimal TotalAmount { get; set; }

        //public List<CartItem>? cartList = Models.CartList.GetCartList() ?? new List<CartItem>();




    }
}
