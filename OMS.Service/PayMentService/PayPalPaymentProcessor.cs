using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using BraintreeHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using OMS.Repositores.DTO;
using OMS.Service.PayMentService;

public class PayPalPaymentProcessor : IPaymentProcessor
{
    private readonly PayPalHttpClient _payPalClient;

    public PayPalPaymentProcessor(PayPalHttpClient client)
    {
        _payPalClient = client;
    }

    public async Task<PaymentResult> ProcessPayment(PaymentDetails paymentDetails)
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
        var statusCode = response.StatusCode;
        var result = response.Result<Order>();

        if (statusCode == HttpStatusCode.Created)
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
            return new PaymentResult
            {
                Success = false,
                Message = "Payment failed"
            };
        }
    }
}
