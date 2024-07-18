using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS.Data.Entites;
using OMS.Data.Entites.Const;
using OMS.Repositores.DTO;
using OMS.Repositores.Interfaces;
using OMS.Service.EmailService;
using OMS.Service.ExceptionsHandeling;
using OMS.Service.Functions;
using OMS.Service.InvoiceService;
using OMS.Service.OrderService;
using System.Security.Claims;

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

        [DynamicFunctionAuthorize("GetAllOrders")]
        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<IReadOnlyList<OrderDTO>>> GetAllOrders()
        {
            var orders = await _unitOfWork.repositories<Order, int>().GetAllAsync(
                include: query => query.Include(o => o.Customer).Include(o => o.OrderItems)
            );

            var orderDtos = _mapper.Map<IReadOnlyList<OrderDTO>>(orders);
            return Ok(orderDtos);
        }

        [DynamicFunctionAuthorize("CreateOrder")]
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<OrderDTO>> CreateOrder([FromBody] CreateOrderDTO createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _unitOfWork.repositories<Customer, int>().GetByIdAsync(createOrderDto.CustomerId);
            if (user == null || user.Email.ToLower() != createOrderDto.Email.ToLower())
            {
                return BadRequest(new ApiResponse(400, "User does not exist"));
            }

            foreach (var item in createOrderDto.OrderItems)
            {
                var productItem = await _unitOfWork.repositories<Product, int>().GetByIdAsync(item.ProductId);
                if (productItem == null) return BadRequest(new ApiResponse(404, "Product not found"));
                if (productItem.Price != item.UnitPrice)
                {
                    item.UnitPrice = productItem.Price;
                }
                if (productItem.Stock < item.Quantity)
                {
                    return BadRequest(new ApiResponse(400, $"{productItem.Name} is out of stock"));
                }
            }

            var order = _mapper.Map<Order>(createOrderDto);
            if (!await _orderService.ValidateStockAsync(order))
            {
                return BadRequest(new ApiResponse(400, "Insufficient stock for one or more products."));
            }

            order.TotalAmount = order.OrderItems.Sum(it => it.Quantity * it.UnitPrice);
            order.Status = OrderStatus.Processing;

            await _unitOfWork.repositories<Order, int>().AddAsync(order);
            await _unitOfWork.CompleteAsync();

            var invoicePath = await _invoiceService.GenerateInvoiceAsync(order);
            await _emailService.SendEmailAsync(order.Customer.Email, "Order Confirmation", invoicePath);

            var orderDto = _mapper.Map<OrderDTO>(order);
            return Ok(orderDto);
        }



        [DynamicFunctionAuthorize("GetOrderById")]
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int orderId)
        {
            var order = await _unitOfWork.repositories<Order, int>().GetByIdAsync(
                id: orderId,
                include: query => query
                        .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product) 
                        .Include(o => o.Customer)
            );

            if (order == null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }

            var orderDto = _mapper.Map<OrderDTO>(order);
            return Ok(orderDto);
        }

        [DynamicFunctionAuthorize("UpdateOrderStatus")]
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromForm] UpdateOrderStatusDTO updateOrderStatusDto)
        {
            var order = await _unitOfWork.repositories<Order, int>().GetByIdAsync(
                id: orderId,
                include: query => query
                        .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                        .Include(o => o.Customer)
            );

            if (order == null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }

            order.Status = updateOrderStatusDto.Status;
            _unitOfWork.repositories<Order, int>().Update(order);
            await _unitOfWork.CompleteAsync();

            var invoicePath = await _invoiceService.GenerateInvoiceAsync(order);

            await _emailService.SendEmailAsync(order.Customer.Email, $"Order Status Updated", invoicePath);

            return NoContent();
        }

    }
}
