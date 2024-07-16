using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OMS.Service.ExceptionsHandeling;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OMS.Data.Middleware
{
    public class ServiceApiMiddleware
    {
        private readonly ILogger<ServiceApiMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public ServiceApiMiddleware
            (
            ILogger<ServiceApiMiddleware> logger,
            RequestDelegate next ,
            IHostEnvironment env
            )
        {
            _logger = logger;
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred processing the request.");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment() ?
                    new CustomException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace) :
                    new CustomException((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.");

                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await context.Response.WriteAsync(json);
            }
        }
    }
}
