using MimeKit;
using Service.Models;

namespace Data.Repository.Interfaces { 
    public interface IInvoiceEmailService
    {
        public void SendEmailInvoice(Message message);
        public void Send(MimeMessage mailMessage);
        public MimeMessage CreateEmailMessage(Message message);
    }
}
