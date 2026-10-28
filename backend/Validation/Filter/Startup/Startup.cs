using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Models.DTOs.Request;

namespace Validation.Startup
{
    public static class Startup
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<ModDto>, ModDtoValidator>();
            services.AddScoped<IValidator<ModEditDto>, ModEditDtoValidator>();

            return services;
        }
    }
}