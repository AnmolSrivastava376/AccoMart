namespace Data.Models.ViewModels
{
    public class CartOrder
    {
       public string userId {  get; set; }  
       public int cartId { get; set; }
       public int addressId {  get; set; }
       public int deliveryId {  get; set; }
    }
}
