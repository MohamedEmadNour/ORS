using OMS.Repositores.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.PayMentService
{
    public interface IPaymentProcessor
    {
        Task<PaymentResult> ProcessPayment(PaymentDetails paymentDetails);
    }
}
