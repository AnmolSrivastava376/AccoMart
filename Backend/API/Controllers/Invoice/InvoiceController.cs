using Microsoft.AspNetCore.Mvc;
using Service.Services.Interface;

namespace API.Controllers.Invoice
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IConfiguration _configuration; 
        public InvoiceController(IInvoiceService invoiceService, IConfiguration configuration)
        {
            _configuration = configuration;
            _invoiceService = invoiceService;
        }

        [HttpPost("GenerateInvoice/{orderId}")]
        public async Task<IActionResult> GenerateInvoice(int orderId)
        {
           await _invoiceService.GenerateInvoiceAsync(orderId);
           return Ok();

        }


        [HttpGet("GetInvoice/{orderId}")]
        public async Task<IActionResult> GetInvoice(int orderId)
        {
            byte[] pdfContent = await _invoiceService.GetInvoiceAsync(orderId);
            string fileName = "Invoice.pdf";
            return File(pdfContent, "application/pdf", fileName);
        }
        }
    
}
