using System.Threading.Tasks;
using Common;

namespace WebFramework.Mail
{
    public interface INotificationService
    {
        Task<bool> SendEmailAsync(SmtpConfig config, string toAddress,string toName,string subject, string messages);
    }
}
