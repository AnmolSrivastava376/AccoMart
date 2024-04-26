﻿namespace API.Models.DTO
{
    public class UpdateProductDto
    {
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}