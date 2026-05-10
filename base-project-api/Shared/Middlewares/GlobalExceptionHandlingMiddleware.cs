using Healthcare.Auth.Api.Shared.Commons;
using Healthcare.Auth.Api.Shared.Commons.Exceptions;
using Healthcare.Auth.Api.Shared.Constants;
using System.Net;
using System.Text.Json;

namespace Healthcare.Auth.Api.Shared.Middlewares
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (BaseApiException ex)
            {
                _logger.LogWarning(ex, "Error controlado: {Message}", ex.Message);
                await WriteResponse(context, ex.StatusCode, ex.PublicMessage, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
                await WriteResponse(context, HttpStatusCode.InternalServerError, ApiMessages.InternalServerError, ex.Message);
            }
        }

        private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string message, string errorDetail)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Fail(message, errorDetail);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
