using Moq;
using OMS.Data.Entites.Const;
using OMS.Repositores.DTO;
using OMS.Service.PayMentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Test.Unit_Testing
{
    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentProcessor> _mockPayPalProcessor;
        private readonly Mock<IPaymentProcessor> _mockStripeProcessor;
        private readonly PaymentService _paymentService;

        public PaymentServiceTests()
        {
            _mockPayPalProcessor = new Mock<IPaymentProcessor>();
            _mockStripeProcessor = new Mock<IPaymentProcessor>();

            var processors = new Dictionary<string, IPaymentProcessor>
        {
            { "paypal", _mockPayPalProcessor.Object },
            { "credit_card", _mockStripeProcessor.Object }
        };

            _paymentService = new PaymentService(processors);
        }

        [Fact]
        public async Task ProcessPayment_ShouldReturnSuccess_ForValidPayPalPayment()
        {
 
            var paymentDetails = new PaymentDetails { Amount = 100, Token = "token" };
            var paymentResult = new PaymentResult { Success = true, TransactionId = "txn123" };

            _mockPayPalProcessor.Setup(p => p.ProcessPayment(paymentDetails))
                .ReturnsAsync(paymentResult);

          
            var result = await _paymentService.ProcessPayment(PaymentMethods.PayPal, paymentDetails);

      
            Assert.True(result.Success);
            Assert.Equal("txn123", result.TransactionId);
        }

    }

}
