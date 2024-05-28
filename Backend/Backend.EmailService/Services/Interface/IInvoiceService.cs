namespace Service.Services.Interface
{
    public interface IInvoiceService
    {
        public Task GenerateInvoiceAsync(int orderId);
        public Task<byte[]> GetInvoiceAsync(int orderId);
    }
}
