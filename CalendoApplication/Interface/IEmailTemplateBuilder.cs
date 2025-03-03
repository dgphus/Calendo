using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoApplication.Interface
{
    public interface IEmailTemplateBuilder
    {
        string BuildEmailTemplate(string title, string bodyContent);
        string BuildConfirmationEmailBody(string email, string callbackUrl);
        string BuildForgotPasswordEmailBody(string email, string callbackUrl);
    }
}
