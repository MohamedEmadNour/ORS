using OMS.Data.Entites;
using OMS.Repositores.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.InvoiceService
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Invoice> GenerateInvoiceAsync(Order order)
        {
            var invoice = new Invoice
            {
                OrderId = order.Id,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = order.TotalAmount
            };

            await _unitOfWork.repositories<Invoice, int>().AddAsync(invoice);
            return invoice;
        }
    }

}
