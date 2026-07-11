using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Common.Errors;
using System.Text.Json;
namespace ClinicFlow.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger,RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {

                _logger.LogError(ex, "An unexpected error occurred in the application.");
                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = 500;

            var result = OperationResult<bool>.Failure(SysteamErrors.Unexpected());

            var jsonResponse = JsonSerializer.Serialize(result);

            return context.Response.WriteAsync(jsonResponse);
        }



    }
}
