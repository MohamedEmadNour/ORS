using Microsoft.Extensions.Configuration;
using OMS.Repositores.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string invoicePath)
        {
            var smtpSettings = _configuration.GetSection("Smtp");
            using var client = new SmtpClient
            {
                Host = smtpSettings["Server"],
                Port = int.Parse(smtpSettings["Port"]),
                EnableSsl = bool.Parse(smtpSettings["UseSsl"]),
                Credentials = new NetworkCredential(smtpSettings["Username"], "uyxhbasvniuflbrs")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["Username"]),
                Subject = subject,
                Body = "Please find your invoice attached.",
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            if (File.Exists(invoicePath))
            {
                mailMessage.Attachments.Add(new Attachment(invoicePath));
            }

            await client.SendMailAsync(mailMessage);
        }
    }

}
