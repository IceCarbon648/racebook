using Business.Interfaces;
using Business.Models.DTOs.Request;
using Business.Models.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Startup
{
    public static class Startup
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IModService, ModService>();
            services.AddScoped<IPlayerStatsSnapshotService, PlayerStatsSnapshotService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFavouriteModService, FavouriteModService>();
            services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<ModDto>, ModDtoValidator>();
            services.AddScoped<IValidator<ModEditDto>, ModEditDtoValidator>();

            return services;
        }
    }
}