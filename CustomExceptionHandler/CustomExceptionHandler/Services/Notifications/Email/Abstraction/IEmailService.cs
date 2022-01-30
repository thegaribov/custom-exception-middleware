using MimeKit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomExceptionHandler.Services.Notifications.Email.Abstraction
{
    public interface IEmailService
    {
        public class Message
        {
            public List<MailboxAddress> Receivers { get; set; } = new List<MailboxAddress>();
            public string Subject { get; set; }
            public string Body { get; set; }

            public Message(List<string> receivers, string subject, string body)
            {
                Receivers.AddRange(receivers.Select(recipent => new MailboxAddress(string.Empty, recipent)));
                Subject = subject;
                Body = body;
            }
        }

        Task<bool> Send(Message message);
    }
}
