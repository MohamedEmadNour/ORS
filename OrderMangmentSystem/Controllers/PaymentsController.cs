using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS.Data.Entites;
using OMS.Repositores.DTO;
using OMS.Repositores.Interfaces;
using OMS.Repositores.Repositories;
using OMS.Service.ExceptionsHandeling;
using OMS.Service.Functions;
using OMS.Service.PayMentService;

namespace OrderMangmentSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentsController(PaymentService paymentService , IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        [DynamicFunctionAuthorize("ProcessPayment")]
        [HttpPost]
        public async Task<ActionResult<PaymentResult>> ProcessPayment([FromBody] PaymentRequest request)
        {
            var order = await _unitOfWork.repositories<Order, int>().GetByIdAsync(
                id: request.OrderId
            );

            if (order is null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }

            var paymentDetails = new PaymentDetails
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Token = request.Token,
            };

            var result = await _paymentService.ProcessPayment(request.Method, paymentDetails);

            if (result.Success)
            {
                // up date the order suatus from frontend and send eamil with th update 
                return Ok(result);
            }

            return BadRequest(result);
        }
    }

}
