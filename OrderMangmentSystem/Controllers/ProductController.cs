using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        
        [HttpGet("GetAllProduct")]
        public async Task<ActionResult<IReadOnlyList<ProductDTO>>> GetAllProduct()
        {
            var products = await _unitOfWork.repositories<Product, int>().GetAllAsync();
            var productMapping = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDTO>>(products);
            return Ok(productMapping);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            if (id != 0)
            {
                var product = await _unitOfWork.repositories<Product, int>().GetByIdAsync(id);
                if (product is null)
                    return NotFound(new ApiResponse(404));

                var productMapping = _mapper.Map<Product, ProductDTO>(product);
                return Ok(productMapping);
            }

            return BadRequest(new ApiResponse(400));
        }

        [HttpPost("AddNewProduct")]
        public async Task<ActionResult<addNewProductDTO>> AddNewProduct([FromForm] addNewProductDTO addNewProductDTO)
        {
            if (ModelState.IsValid)
            {
                var product = _mapper.Map<addNewProductDTO, Product>(addNewProductDTO);
                if (product is not null)
                {
                    var result = _unitOfWork.repositories<Product, int>().AddAsync(product);
                    if (result.IsCompletedSuccessfully)
                    {
                        return Ok(result);
                    }
                }
            }
            return BadRequest(addNewProductDTO);
        }

        [HttpPut("UpDateProduct")]
        public async Task<ActionResult<Product>> UpDateProduct([FromForm] Product addNewProductDTO)
        {
            if (addNewProductDTO.Id != 0)
            {
                var product = _unitOfWork.repositories<Product, int>().GetByIdAsync(addNewProductDTO.Id);
                if (product is null) return NotFound(new ApiResponse(404));

                var productUpdeted = new Product()
                {
                    Id = product.Id,
                    Name = addNewProductDTO.Name,
                    Price = addNewProductDTO.Price,
                    Stock = addNewProductDTO.Stock,
                };

                _unitOfWork.repositories<Product, int>().Update(productUpdeted);

                return Ok(productUpdeted);
            }
            return BadRequest(addNewProductDTO);
        }






    }
}
