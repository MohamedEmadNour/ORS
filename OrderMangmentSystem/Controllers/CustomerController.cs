using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS.Data.Entites;
using OMS.Repositores.DTO;
using OMS.Repositores.Interfaces;
using OMS.Service.ExceptionsHandeling;
using OMS.Service.Functions;
using System.Threading.Tasks;

namespace OrderMangmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [DynamicFunctionAuthorize("CreateCustomer")]
        [HttpPost]
        public async Task<ActionResult<CustomerDTO>> CreateCustomer([FromBody] CreateCustomerDTO createCustomerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = _mapper.Map<Customer>(createCustomerDTO);
            var result = _unitOfWork.repositories<Customer, int>().AddAsync(customer);
            if (result.IsCompletedSuccessfully)
            {
                await _unitOfWork.CompleteAsync();
                var customerDTO = _mapper.Map<CustomerDTO>(customer);
                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customerDTO);
                
            }

            return NotFound(new ApiResponse(500));
        }

        [DynamicFunctionAuthorize("GetAllOrdersForCustomer")]
        [HttpGet("{customerId}/orders")]
        public async Task<ActionResult<IReadOnlyList<OrderDTO>>> GetAllOrdersForCustomer(int customerId)
        {
            var customer = await GetCustomerById(customerId);
            if (customer == null)
            {
                return NotFound(new ApiResponse(404, "Customer not found"));
            }

            var orders = await _unitOfWork.repositories<Order, int>().GetAllAsync(
                predicate : o => o.CustomerId == customerId,
                include: o => o.Include( o => o.OrderItems ) 
            );

            var orderMapping = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderDTO>>(orders);
            
            return Ok(orderMapping);
    
        }

        [DynamicFunctionAuthorize("GetCustomerById")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomerById(int id)
        {
            if (id != 0)
            {
                var customer = await _unitOfWork.repositories<Customer, int>().GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound(new ApiResponse(404, "Customer not found"));
                }

                var customerDTO = _mapper.Map<CustomerDTO>(customer);
                return Ok(customerDTO);
            }

            return BadRequest(ModelState);
        }




    }
}
