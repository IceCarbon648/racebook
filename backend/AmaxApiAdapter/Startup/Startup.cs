using AmaxApiAdapter.Adapters;
using AmaxApiAdapter.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AmaxApiAdapter.Startup
{
    public static class Startup
    {
        public static IServiceCollection AddAmax(this IServiceCollection services)
        {
            services.AddHttpClient<IAmaxHttpClient, AmaxHttpClient>();
            services.AddScoped<IAmaxAdapter, AmaxAdapter>();

            return services;
        }
    }
}