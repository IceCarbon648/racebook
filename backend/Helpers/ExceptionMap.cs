using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Collections.Frozen;

namespace Business.Helpers
{
    public static class ExceptionMap
    {
        public static readonly (int StatusCode, string Title, string Detail, string TypeUrl) DefaultResponse =
        (
            StatusCodes.Status500InternalServerError,
            "Unexpected Error",
            "An unexpected error occurred while processing your request.",
            "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        );

        public static readonly FrozenDictionary<Type, (int StatusCode, string Title, string Detail, string TypeUrl)> ExceptionToResponse =
        new Dictionary<Type, (int, string, string, string)>
        {
            {
                typeof(ArgumentException),
                (StatusCodes.Status400BadRequest,
                 "Bad Request",
                 "One or more arguments provided were invalid.",
                 "https://tools.ietf.org/html/rfc7231#section-6.5.1")
            },
            {
                typeof(ArgumentNullException),
                (StatusCodes.Status400BadRequest,
                 "Bad Request",
                 "A required input was null or missing.",
                 "https://tools.ietf.org/html/rfc7231#section-6.5.1")
            },
            {
                typeof(UnauthorizedAccessException),
                (StatusCodes.Status401Unauthorized,
                 "Unauthorized",
                 "You do not have permission to access or modify this resource.",
                 "https://tools.ietf.org/html/rfc7231#section-6.5.3")
            },
            {
                typeof(KeyNotFoundException),
                (StatusCodes.Status404NotFound,
                 "Not Found",
                 "The requested resource does not exist.",
                 "https://tools.ietf.org/html/rfc7231#section-6.5.4")
            },
            {
                typeof(InvalidOperationException),
                (StatusCodes.Status409Conflict,
                 "Conflict",
                 "The request could not be completed due to a conflict.",
                 "https://tools.ietf.org/html/rfc7231#section-6.5.8")
            },
            {
                typeof(ValidationException),
                (StatusCodes.Status400BadRequest,
                 "Validation Failed",
                 "One or more fields failed validation.",
                 "https://tools.ietf.org/html/rfc7231#section-6.5.1")
            },
            {
                typeof(IOException),
                (StatusCodes.Status500InternalServerError,
                 "I/O Error",
                 "An unexpected error occurred while accessing storage.",
                 "https://tools.ietf.org/html/rfc7231#section-6.6.1")
            }
        }.ToFrozenDictionary();
    }
}