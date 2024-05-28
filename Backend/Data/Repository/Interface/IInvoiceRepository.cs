
namespace Data.Repository.Interface
{
    public interface IInvoiceRepository
    {
        Task GenerateInvoice(int orderId);
        Task<byte[]> GetInvoice(int orderId);
    }
}
