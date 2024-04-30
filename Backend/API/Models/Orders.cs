namespace API.Models
{
    public class Orders
    {
        public int OrderId { get; set; } 
        public DateTime OrderDate { get; set; }

        public TimeSpan OrderTime { get; set; }

        public int AddressId { get; set; }

        public int UserId { get; set; }
    }
}
