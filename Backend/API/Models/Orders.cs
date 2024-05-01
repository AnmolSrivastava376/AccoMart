﻿namespace API.Models
{
    public class Orders
    {
        public int OrderId { get; set; } 
        public DateTime OrderDate { get; set; }
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public float OrderAmount {  get; set; }
        public TimeOnly OrderTime { get; set; }

        public int ProductId { get; set; }

        public int CartId { get; set; }

        public int DeliveryServiceID { get; set; }




    }
}