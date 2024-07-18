using Microsoft.Extensions.Logging;
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
        private readonly ILogger<StripePaymentProcessor> _logger;

        public StripePaymentProcessor(string apiKey, ILogger<StripePaymentProcessor> logger)
        {
            _stripeClient = new StripeClient(apiKey);
            _logger = logger;
        }

        public async Task<PaymentResult> ProcessPayment(PaymentDetails paymentDetails)
        {
            try
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt64(paymentDetails.Amount),
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe payment");
                return new PaymentResult
                {
                    Success = false,
                    Message = "Payment failed"
                };
            }
        }
    }

}
