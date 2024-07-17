using OMS.Repositores.DTO;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.PayMentService
{
    public class StripePaymentProcessor : IPaymentProcessor
    {
        private readonly StripeClient _stripeClient;

        public StripePaymentProcessor(string apiKey)
        {
            _stripeClient = new StripeClient(apiKey);
        }

        public async Task<PaymentResult> ProcessPayment(PaymentDetails paymentDetails)
        {
            var options = new ChargeCreateOptions
            {
                Amount = paymentDetails.Amount,
                Currency = "usd",
                Source = paymentDetails.Token, 
                Description = "Payment Description",
            };

            var service = new ChargeService(_stripeClient);
            Charge charge = await service.CreateAsync(options);

            return new PaymentResult
            {
                Success = charge.Status == "succeeded",
                TransactionId = charge.Id,
                Message = charge.Outcome.SellerMessage
            };
        }
    }

}
