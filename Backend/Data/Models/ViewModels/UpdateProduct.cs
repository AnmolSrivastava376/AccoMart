﻿namespace Data.Models.ViewModels.UpdateProduct
{
    public class UpdateProduct
    {
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; }
        public int CategoryId { get; set; }
        public int Stock {  get; set; }
    }
}
