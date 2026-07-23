using Business.Helpers;
using Microsoft.AspNetCore.Diagnostics;

namespace racebookApi.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
        {
            var (status, title, detail, type) =
                ExceptionMap.ExceptionToResponse.TryGetValue(exception.GetType(), out var value)
                ? value
                : ExceptionMap.DefaultResponse;

            httpContext.Response.StatusCode = status;
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = new
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