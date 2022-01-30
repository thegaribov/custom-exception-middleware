using CustomExceptionHandler.Services.Notifications.Email.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace CustomExceptionHandler.Services.Notifications.Email.Implementation.SMTP
{
    public class Smtp : IEmailService
    {
        ///If your SMTP host is gmail then don't forget to
        ///enable "Less secure app access" from settings
        private readonly string _senderEmail;
        private readonly string _senderEmailPassword;
        private readonly string _senderHost;
        private readonly int _port;

        private readonly ILogger<Smtp> _logger;

        public Smtp(ILogger<Smtp> logger)
        {
            _senderEmail = Environment.GetEnvironmentVariable("SMTP_SENDER_EMAIL");
            _senderEmailPassword = Environment.GetEnvironmentVariable("SMTP_SENDER_EMAIL_PASSWORD");
            _senderHost = Environment.GetEnvironmentVariable("SMTP_SENDER_HOST");
            _port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));

            _logger = logger;
        }

        public async Task<bool> Send(IEmailService.Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(string.Empty, _senderEmail));
            emailMessage.To.AddRange(message.Receivers);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Body };

            return SendEmail(emailMessage);
        }

        private bool SendEmail(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_senderHost, _port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_senderEmail, _senderEmailPassword);

                    var result = client.Send(mailMessage);

                    return true;
                }
                catch (Exception smtpException)
                {
                    _logger.LogError(smtpException, $"[{nameof(Smtp)}] [UTC] [{DateTime.UtcNow.ToString("dd/MM/yyy HH:mm:ss")}] Send email unsuccessfully completed, exception occurred.");

                    return false;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
