using MimeKit;
using System.Net.Mail;

namespace Service.Models
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<(string fileName, byte[] content, string contentType)>? Attachments { get; set; }

        public Message(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("email", x)));
            Subject = subject;
            Content = content;
            Attachments = new List<(string, byte[], string)>();
        }

        public void AddAttachment(string fileName, byte[] content, string contentType)
        {
            Attachments.Add((fileName, content, contentType));
        }

    }
}
