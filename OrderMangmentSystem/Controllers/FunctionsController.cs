using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OMS.Data.Entites;
using OMS.Data.Entites.System;
using OMS.Repositores.DTO;
using OMS.Repositores.Interfaces;
using OMS.Service.ExceptionsHandeling;
using OMS.Service.Functions;

namespace OrderMangmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionsController : ControllerBase
    {
        private readonly IFunctionService _functionService;

        public FunctionsController(IFunctionService functionService)
        {
            _functionService = functionService;
        }

        [DynamicFunctionAuthorize("UpdateFunctions")]
        [HttpPut("update-functions")]
        public async Task<ActionResult> UpdateFunctions([FromBody] List<FunctionDisplayDTO> functionDtos)
        {
            if (functionDtos == null || !functionDtos.Any())
            {
                return BadRequest(new ApiResponse(400, "No functions to update."));
            }

            var functions = await _functionService.GetAllFunctionsAsync();
            if (functions == null)
            {
                return NotFound();
            }

            var functionLookup = functions.ToDictionary(f => f.tbFunctionsId);

            foreach (var functionDto in functionDtos)
            {
                if (functionLookup.TryGetValue(functionDto.FunctionId, out var function))
                {
                    function.IsAdminFunction = functionDto.IsAdminFunction;
                    function.IsUserFunction = functionDto.IsUserFunction;
                }
            }

            await _functionService.UpdateFunctionsAsync(functions);
            return NoContent();
        }

        [DynamicFunctionAuthorize("GetAllFunctions")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<FunctionDisplayDTO>>> GetAllFunctions()
        {
            var functions = await _functionService.GetAllFunctionsAsync();
            var functionDtos = functions.Select(f => new FunctionDisplayDTO
            {
                FunctionId = f.tbFunctionsId,
                FunctionName = f.FunctionName,
                IsAdminFunction = f.IsAdminFunction,
                IsUserFunction = f.IsUserFunction
            }).ToList();

            return Ok(functionDtos);
        }
    }
}
