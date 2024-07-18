using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS.Data.Entites;
using OMS.Repositores.DTO;
using OMS.Repositores.Interfaces;
using OMS.Repositores.Repositories;
using OMS.Service.EmailService;
using OMS.Service.ExceptionsHandeling;
using OMS.Service.Functions;
using OMS.Service.InvoiceService;
using OMS.Service.PayMentService;
using Stripe;

namespace OrderMangmentSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceService _invoiceService;
        private readonly IEmailService _emailService;
        const string endpointSecret = "whsec_d439ab9aaa815f0dcb786a4c4c760e0effd4e8fb03111c45aa3b98ef89b9f5e7";

        public PaymentsController(PaymentService paymentService
                        , IUnitOfWork unitOfWork , IEmailService emailService , IInvoiceService invoiceService )
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _invoiceService = invoiceService;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<ActionResult<PaymentResult>> ProcessPayment([FromForm] PaymentRequest request)
        {
            var order = await _unitOfWork.repositories<Order, int>().GetByIdAsync(
                id: request.OrderId,
                include: query => query
                        .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                        .Include(o => o.Customer)
            );


            if (order == null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }

            var paymentDetails = new PaymentDetails
            {
                OrderId = request.OrderId,
                Amount = order.TotalAmount,
                Token = request.Token,
            };

            var result = await _paymentService.ProcessPayment(request.Method, paymentDetails);

            if (result.Success)
            {
                var invoicePath = await _invoiceService.GenerateInvoiceAsync(order);

                await _emailService.SendEmailAsync(order.Customer.Email, $"Order Confirmation", invoicePath);
                return Ok(result);
            }

            return BadRequest(result);
        }
    }


}
