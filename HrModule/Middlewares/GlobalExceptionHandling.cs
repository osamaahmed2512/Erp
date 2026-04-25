using Application.Dtos.Response;
using System.Net;
using System.Text.Json;

namespace HrModule.Middlewares
{
    public class GlobalExceptionHandling
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandling> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandling(RequestDelegate next, ILogger<GlobalExceptionHandling> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            object response = _env.IsDevelopment()
            ?
            new DevelopmentExceptionResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                InnerException = ex.InnerException?.Message,
                InnerStackTrace = ex.InnerException?.StackTrace,
                Path = context.Request.Path,
                Timestamp = DateTime.UtcNow
            }
             : new ProductionExceptionResponse
             {
                 StatusCode = context.Response.StatusCode,
                 Message = "An unexpected error occurred. Please contact support.",
                 Path = context.Request.Path,
                 Timestamp = DateTime.UtcNow
             };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }    
    
}
