namespace Data.Models.Statistic_Models
{
    public class OrderQuantity
    {
        public int Count { get; set; }
        public DateTime OrderDate { get; set; }
        public long TotalSales { get; set; } = 0;
    }
}
