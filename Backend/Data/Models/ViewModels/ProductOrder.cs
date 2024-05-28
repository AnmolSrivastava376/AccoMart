namespace Data.Models.ViewModels
{
    public class ProductOrder
    {
        public string UserId { get; set; }
        public int AddressId { get; set; }
        public int DeliveryId{ get; set; }
        public int ProductId { get; set; }
        public int quantity   {  get; set; }

    }
}
