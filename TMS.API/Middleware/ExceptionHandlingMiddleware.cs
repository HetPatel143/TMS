using System.Net;
using System.Text.Json;
using TMS.API.Models;
using TMS.Core.Exceptions;

namespace TMS.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An internal server error occurred.";
            var errors = new List<string>();

            switch (exception)
            {
                case NotFoundException notFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundEx.Message;
                    _logger.LogWarning(notFoundEx, "Resource not found: {Message}", notFoundEx.Message);
                    break;

                case UnauthorizedException unauthorizedEx:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = unauthorizedEx.Message;
                    _logger.LogWarning(unauthorizedEx, "Unauthorized access: {Message}", unauthorizedEx.Message);
                    break;

                case ValidationException validationEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = validationEx.Message;
                    errors = validationEx.ErrorList;
                    _logger.LogWarning(validationEx, "Validation failed: {Message}", validationEx.Message);
                    break;

                default:
                    _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new ApiResponse
            {
                Success = false,
                Message = message,
                Data = null,
                Errors = errors
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
