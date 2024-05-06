using Microsoft.AspNetCore.Mvc;
using Service.Services.Interface;

namespace API.Controllers.Invoice
{
    public class InvoiceController : Controller
    {
        private readonly ICartService _cartService;
        public InvoiceController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> GenerateInvoice(int orderId)
        {
           await _cartService.GenerateInvoiceAsync(orderId);
           return Ok();

        }

        public async Task<IActionResult>GetInvoice(int orderId)
        {
            await _cartService.GetInvoiceAsync(orderId);
            return Ok();
        }
    }
}
