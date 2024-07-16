using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OMS.Data.Entites;
using OMS.Repositores.DTO;
using OMS.Repositores.Interfaces;
using OMS.Service.EmailService;
using OMS.Service.ExceptionsHandeling;
using OMS.Service.Functions;
using OMS.Service.InvoiceService;
using OMS.Service.OrderService;

namespace OrderMangmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly IInvoiceService _invoiceService;
        private readonly IEmailService _emailService;

        public OrderController(IUnitOfWork unitOfWork, IMapper mapper , IOrderService orderService, IInvoiceService invoiceService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _orderService = orderService;
            _invoiceService = invoiceService;
            _emailService = emailService;
        }

        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<IReadOnlyList<OrderDTO>>> GetAllOrders()
        {
            var orders = await _unitOfWork.repositories<Order, int>().GetAllAsync();
            var orderDtos = _mapper.Map<IReadOnlyList<OrderDTO>>(orders);
            return Ok(orderDtos);
        }

        [HttpPost("CreateOrder")]
        public async Task<ActionResult<OrderDTO>> CreateOrder(CreateOrderDTO createOrderDto)
        {
            var order = _mapper.Map<Order>(createOrderDto);

            if (!await _orderService.ValidateStockAsync(order))
            {
                return BadRequest(new ApiResponse(400, "Insufficient stock for one or more products."));
            }

            order.TotalAmount = _orderService.ApplyDiscount(order);
            await _unitOfWork.repositories<Order, int>().AddAsync(order);

            var invoice = await _invoiceService.GenerateInvoiceAsync(order);

            await _emailService.SendEmailAsync(order.Customer.Email, "Order Confirmation", "Your order has been placed.");


            var orderDto = _mapper.Map<OrderDTO>(order);
            return CreatedAtRoute("GetOrderById", new { orderId = order.Id }, orderDto);
        }



        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int orderId)
        {
            var order = await _unitOfWork.repositories<Order, int>().GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }

            var orderDto = _mapper.Map<OrderDTO>(order);
            return Ok(orderDto);
        }

        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, UpdateOrderStatusDTO updateOrderStatusDto)
        {
            var order = await _unitOfWork.repositories<Order, int>().GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }

            order.Status = updateOrderStatusDto.Status;
            _unitOfWork.repositories<Order, int>().Update(order);

            await _emailService.SendEmailAsync(order.Customer.Email, order.Status.ToString() , "Your order has been placed.");


            return NoContent();
        }
    }
}
