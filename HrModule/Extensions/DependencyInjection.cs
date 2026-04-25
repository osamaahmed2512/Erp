using Application;
using Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace HrModule.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationServices(configuration);
            services.AddInfrastructureServices(configuration);
            services.Configure<DataProtectionTokenProviderOptions>(options => {
                options.TokenLifespan = TimeSpan.FromHours(1);
            });

            return services;
        }
    }
}
