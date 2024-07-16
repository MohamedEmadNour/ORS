using OMS.Data.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.InvoiceService
{
    public interface IInvoiceService
    {
        Task<Invoice> GenerateInvoiceAsync(Order order);
    }
}
