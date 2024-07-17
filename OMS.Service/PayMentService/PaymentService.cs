using OMS.Repositores.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.PayMentService
{
    public class PaymentService
    {
        private readonly Dictionary<string, IPaymentProcessor> _paymentProcessors;

        public PaymentService(Dictionary<string, IPaymentProcessor> paymentProcessors)
        {
            _paymentProcessors = paymentProcessors;
        }

        public async Task<PaymentResult> ProcessPayment(string method, PaymentDetails paymentDetails)
        {
            if (_paymentProcessors.TryGetValue(method, out var processor))
            {
                return await processor.ProcessPayment(paymentDetails);
            }

            throw new NotSupportedException($"Payment method {method} is not supported.");
        }
    }

}
