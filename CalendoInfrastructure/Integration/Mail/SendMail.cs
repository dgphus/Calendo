using CalendoApplication.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CalendoInfrastructure.Integration.Mail
{
    public class SendMail : ISendMail
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailTemplateBuilder _emailTemplateBuilder;
        private readonly IUnitOfWork _unitOfWork;
        public SendMail(IConfiguration configuration, IEmailTemplateBuilder emailTemplateBuilder, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _emailTemplateBuilder = emailTemplateBuilder;
            _unitOfWork = unitOfWork;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:SmtpUsername"], _configuration["EmailSettings:SmtpPassword"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:SmtpUsername"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendConfirmationEmailAsync(string email, string callbackUrl)
        {
            var emailTemplateBuilder = new EmailTemplateBuilder();
            var emailBody = emailTemplateBuilder.BuildConfirmationEmailBody(email, callbackUrl);

            await SendEmailAsync(email, "Xác nhận tài khoản", emailBody);
        }

    }
}
