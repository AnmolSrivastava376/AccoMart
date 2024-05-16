using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interface
{
    public interface IInvoiceService
    {
        public Task GenerateInvoiceAsync(int orderId);
        public Task<byte[]> GetInvoiceAsync(int orderId);
    }
}
