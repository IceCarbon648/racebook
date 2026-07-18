using Microsoft.Extensions.DependencyInjection;
using Business.Interfaces;

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

            return services;
        }
    }
}