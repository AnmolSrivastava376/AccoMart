using MimeKit;
using Service.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Data.Repository.Interfaces;

namespace Data.Repository.Implementation
{

    public class InvoiceEmailService : IInvoiceEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        public InvoiceEmailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        
        public  void SendEmailInvoice(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            var builder = new BodyBuilder();
            builder.HtmlBody = message.Content;
            foreach (var attachment in message.Attachments)
            {
                builder.Attachments.Add(attachment.fileName, attachment.content, ContentType.Parse(attachment.contentType));
            }

            emailMessage.Body = builder.ToMessageBody();

            Send(emailMessage);

        }


        public MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = message.Content
            };
            return emailMessage;
        }


        public void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();

            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);

                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }

        }


    }
}
