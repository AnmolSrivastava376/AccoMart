using MimeKit;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interface
{
    public interface IEmailService
    {
        public void SendEmail(Message message);

        public void Send(MimeMessage mailMessage);
        public MimeMessage CreateEmailMessage(Message message);


    }
}
