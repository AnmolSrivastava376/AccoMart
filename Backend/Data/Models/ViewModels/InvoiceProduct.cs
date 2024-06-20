﻿using Service.Models;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.ViewModels
{
    public class InvoiceProduct
    {
        [Required(ErrorMessage = "Product Name is requierd")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Product description is required")]
        public string ProductDesc { get; set; }
        [Required(ErrorMessage ="Product price is required"), Range(1, Int32.MaxValue)]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage ="Quantity is requied"), Range(1, Int32.MaxValue)]
        public int Quantity { get; set; }
    }
}
