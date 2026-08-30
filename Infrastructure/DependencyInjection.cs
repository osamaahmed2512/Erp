using Infrastructure.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Application.Dtos.Token;
using Application.Interfaces.ExternalServices;
using Infrastructure.Services;
using Domain.Interfaces.UnitOfWork;
using Infrastructure.UnitOfWork;
using Domain.Entities;
using Infrastructure.Setting;
using Application.Interfaces.InternalServices;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                 options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();
           
            services.AddAuthentication(

                    options => {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.SaveToken = true;
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = configuration["Jwt:Issuer"],
                            ValidAudience = configuration["Jwt:Audience"],
                            ClockSkew = TimeSpan.Zero,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                        };
                    }
             );
            services.Configure<TokenConfig>(configuration.GetSection("Jwt"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IUnitOfWork, Infrastructure.UnitOfWork.UnitOfWork>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddHttpContextAccessor();
            services.AddAuthorization(options =>
            {
                foreach (var permission in Permissions.GetAll())
                {
                    options.AddPolicy(permission, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.AddRequirements(new PermissionRequirement(permission));
                    });
                }
            });
            return services;
        }
    }
}
