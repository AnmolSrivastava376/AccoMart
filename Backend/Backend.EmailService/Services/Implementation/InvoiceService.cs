using Data.Repository.Implementation;
using Data.Repository.Interface;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Implementation
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }
        async Task IInvoiceService.GenerateInvoiceAsync(int cartId)
        {
            await _invoiceRepository.GenerateInvoice(cartId);
        }

        async Task<byte[]> IInvoiceService.GetInvoiceAsync(int orderId)
        {
           return await _invoiceRepository.GetInvoice(orderId);
        }
    }
}
