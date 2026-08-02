using Business.Helpers;
using Microsoft.AspNetCore.Diagnostics;

namespace racebookApi.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred.");

            (int status, string title, string detail, string type) =
                ExceptionMap.ExceptionToResponse.TryGetValue(exception.GetType(), out (int, string, string, string) value)
                ? value
                : ExceptionMap.DefaultResponse;

            httpContext.Response.StatusCode = status;
            httpContext.Response.ContentType = "application/problem+json";

            object problemDetails = new
            {
                type,
                title,
                status,
                detail,
                instance = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}