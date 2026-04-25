using Microsoft.OpenApi.Models;

namespace HrModule
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentationServices(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ERP API",
                    Version = "v1",
                    Description = "API documentation for ERP with JWT authentication support"
                });
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter JWT token like: **Bearer your_token_here**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securityScheme);
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                };
                c.AddSecurityRequirement(securityRequirement);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllWithCredentials", policy =>
                {
                    policy
                        .WithOrigins(

                                                 "http://localhost:4200",
                                                 "https://admin.sendepad.com",
                                                 "http://admin-sendepad.runasp.net"

                                             )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            return services;
        }
    }
}
