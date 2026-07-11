using Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Startup
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ICloudinaryRepository, CloudinaryRepository>();
            services.AddScoped<IModRepository, ModRepository>();
            services.AddScoped<IPlayerStatsSnapshotRepository,PlayerStatsSnapshotRepository>();
            services.AddScoped<IPreviewImageRepository, PreviewImageRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}