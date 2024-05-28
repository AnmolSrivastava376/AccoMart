using MimeKit;
using Service.Models;

namespace Service.Services.Interface
{
    public interface IEmailService
    {
        public void SendEmail(Message message);
        public void Send(MimeMessage mailMessage);
        public MimeMessage CreateEmailMessage(Message message);
    }
}
