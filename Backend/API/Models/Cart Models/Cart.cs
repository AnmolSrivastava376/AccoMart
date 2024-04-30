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

        static readonly List<CartItem> cart = new List<CartItem> { };   
        public static List<CartItem> GetCartList()
        {
            return cart;
        }
    }
}
