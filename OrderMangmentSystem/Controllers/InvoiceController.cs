using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OMS.Data.Entites;
using OMS.Repositores.DTO;
using OMS.Repositores.Interfaces;
using OMS.Service.ExceptionsHandeling;
using OMS.Service.Functions;

namespace OrderMangmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InvoiceController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [DynamicFunctionAuthorize("GetInvoiceById")]
        [HttpGet("{invoiceId}", Name = "GetInvoiceById")]
        public async Task<ActionResult<InvoiceDTO>> GetInvoiceById(int invoiceId)
        {
            var invoice = await _unitOfWork.repositories<Invoice, int>().GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound(new ApiResponse(404, "Invoice not found"));
            }

            var invoiceDTO = _mapper.Map<InvoiceDTO>(invoice);
            return Ok(invoiceDTO);
        }

        [DynamicFunctionAuthorize("GetAllInvoices")]
        [HttpGet("GetAllInvoices")]
        public async Task<ActionResult<IReadOnlyList<InvoiceDTO>>> GetAllInvoices()
        {
            var invoices = await _unitOfWork.repositories<Invoice, int>().GetAllAsync();
            var invoiceDTOs = _mapper.Map<IReadOnlyList<InvoiceDTO>>(invoices);
            return Ok(invoiceDTOs);
        }
    }
}
