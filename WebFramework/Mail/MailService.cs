using System;
using System.Threading.Tasks;
using Common;
using MimeKit;

namespace WebFramework.Mail
{
    public class MailService : INotificationService
    {
        public async Task<bool> SendEmailAsync(SmtpConfig config, string toAddress, string toName, string subject, string messages)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(config.FromName, config.FromAddress));
                message.To.Add(new MailboxAddress(toName, toAddress));
                message.Subject = subject;

                message.Body = new TextPart("plain")
                {
                    Text = messages
                };

                using var client = new MailKit.Net.Smtp.SmtpClient
                {
                    ServerCertificateValidationCallback = (s, c, h, e) => true
                };

                client.Connect(config.Server, config.Port, config.UseSsl);
                client.Authenticate(config.Username, config.Password);
                client.Disconnect(true);
                client.Send(message);
                
                await client.SendAsync(message);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
