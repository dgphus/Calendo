using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoApplication.Interface
{
    public interface ISendMail
    {
        Task SendEmailAsync(string toEmail, string subject, string message);

        Task SendConfirmationEmailAsync(string email, string callbackUrl);

        Task SendForgotPasswordEmailAsync(string email, string callbackUrl);
    }
}
