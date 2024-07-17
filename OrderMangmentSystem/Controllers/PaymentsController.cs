using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OMS.Repositores.DTO;
using OMS.Service.PayMentService;

namespace OrderMangmentSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<PaymentResult>> ProcessPayment([FromBody] PaymentRequest request)
        {
            var paymentDetails = new PaymentDetails
            {
                Amount = request.Amount,
                Token = request.Token,
            };

            var result = await _paymentService.ProcessPayment(request.Method, paymentDetails);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }

}
