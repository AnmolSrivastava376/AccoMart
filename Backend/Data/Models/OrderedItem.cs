namespace Data.Models
{
    public class OrderedItem
    {
        public int OrderId { get; set; }
        public int ProductID { get; set; }

        public int   OrderedQuantity { get; set; }    
        public float OrderedAmount { get; set; }
    }
}
