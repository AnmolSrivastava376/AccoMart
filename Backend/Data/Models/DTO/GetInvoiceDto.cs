﻿using Data.Models.CartModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Data.Models.DTO
{
    public class GetInvoiceDto
    {
        public int InvoiceId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }   
        public string PhoneNumber { get; set; } 
        public AddressModel Address { get; set; }         
        public List<InvoiceProductDto>? products { get; set; }    
        public DateTime OrderDate {  get; set; } 
        public DateTime OrderTime { get; set;}
        public float OrderAmount {  get; set; } 





    }
}