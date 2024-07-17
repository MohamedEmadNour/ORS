using OMS.Repositores.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string invoicePath);
    }
}
