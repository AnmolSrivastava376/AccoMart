namespace API.Models
{
    public  class Cart
    {
       /* public string CartId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public int DeliveryId { get; set; }
        //public string ClientSecret { get; set; }
        public string PaymentId { get; set; }
        public decimal ShippingPrice { get; set; } */

        public int CartId { get; set; } 
        public int UserId {  get; set; }
        public int CartItemId { get; set; }

        
    }
}
