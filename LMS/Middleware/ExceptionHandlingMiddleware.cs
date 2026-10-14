using System.Net;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

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
                var (statusCode, message) = MapException(ex);

                if (statusCode == HttpStatusCode.InternalServerError)
                {
                    _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
                }
                else
                {
                    _logger.LogWarning(ex, "Request failed with {StatusCode}. TraceId: {TraceId}", (int)statusCode, context.TraceIdentifier);
                }

                await WriteResponseAsync(context, statusCode, message);
            }
        }

        // خطاهای قابل‌پیش‌بینی دیتابیس را به پیام روشن تبدیل می‌کند
        private static (HttpStatusCode StatusCode, string Message) MapException(Exception exception)
        {
            if (exception is DbUpdateException { InnerException: SqlException sqlException })
            {
                switch (sqlException.Number)
                {
                    case 547: // نقض Foreign Key
                        return (HttpStatusCode.Conflict,
                            "This item cannot be deleted or changed because other records depend on it.");
                    case 2601: // نقض Unique Index
                    case 2627: // نقض Unique Constraint
                        return (HttpStatusCode.Conflict,
                            "A record with the same value already exists.");
                }
            }

            return (HttpStatusCode.InternalServerError,
                "An unexpected error occurred. Please try again later.");
        }

        private static Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new ErrorResponseDto
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
                TraceId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(response, JsonOptions);
            return context.Response.WriteAsync(json);
        }
    }
}