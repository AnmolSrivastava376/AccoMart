namespace Data.Models.DTO
{
    public class CartOrderDto
    {
       public string userId {  get; set; }  
       public int cartId { get; set; }
       public int addressId {  get; set; }
       public int deliveryId {  get; set; }
    }
}
