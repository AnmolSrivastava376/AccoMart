namespace API.Models.DTO
{
    public class CartOrderDto
    {
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public int CartId { get; set; }
        public int DeliveryServiceID { get; set; }
    }
}
