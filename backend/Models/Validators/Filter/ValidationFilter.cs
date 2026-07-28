using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Models.Validators.Filter
{
    public class ValidationFilter<T> : IAsyncActionFilter where T : class
    {
        private readonly IValidator<T> _validator;

        public ValidationFilter(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            T? dto = context.ActionArguments.Values.OfType<T>().FirstOrDefault();

            if (dto is null)
            {
                context.Result = new BadRequestObjectResult("Invalid request");

                return;
            }

            ValidationResult result = await _validator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                context.Result = new BadRequestObjectResult(result.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    error = e.ErrorMessage
                }));

                return;
            }

            await next();
        }
    }
}