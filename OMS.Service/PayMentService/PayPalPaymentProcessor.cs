using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using BraintreeHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using OMS.Repositores.DTO;
using OMS.Service.PayMentService;
using Microsoft.Extensions.Logging;

namespace OMS.Service.PayMentService
{
    public class PayPalPaymentProcessor : IPaymentProcessor
    {
        private readonly PayPalHttpClient _payPalClient;
        private readonly ILogger<PayPalPaymentProcessor> _logger;

        public PayPalPaymentProcessor(PayPalHttpClient client, ILogger<PayPalPaymentProcessor> logger)
        {
            _payPalClient = client;
            _logger = logger;
        }

        public async Task<PaymentResult> ProcessPayment(PaymentDetails paymentDetails)
        {
            try
            {
                var request = new OrdersCreateRequest();
                request.Prefer("return=representation");
                request.RequestBody(new OrderRequest()
                {
                    CheckoutPaymentIntent = "CAPTURE",
                    PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "USD",
                            Value = paymentDetails.Amount.ToString()
                        }
                    }
                }
                });

                var response = await _payPalClient.Execute(request);
                var result = response.Result<Order>();

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return new PaymentResult
                    {
                        Success = true,
                        TransactionId = result.Id,
                        Message = "Payment successful"
                    };
                }
                else
                {
                    _logger.LogError("PayPal payment failed: {Response}", response);
                    return new PaymentResult
                    {
                        Success = false,
                        Message = "Payment failed"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PayPal payment");
                return new PaymentResult
                {
                    Success = false,
                    Message = "Payment failed"
                };
            }
        }
    }
}

