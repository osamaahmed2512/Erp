using Domain.Entities;
using Domain.Interfaces.UnitOfWork;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seeders
{
    public static class AppSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var uow = services.GetRequiredService<IUnitOfWork>();
            var db = services.GetRequiredService<AppDbContext>();
            var configuration = services.GetRequiredService<IConfiguration>();

            await RoleSeeder.SeedAsync(roleManager);
            var rootEmail = configuration["AccessControl:RootSuperAdminEmail"]
                ?? throw new InvalidOperationException("AccessControl:RootSuperAdminEmail is required.");
            await AdminSeeder.SeedAsync(userManager, rootEmail);
            await AccessControlSeeder.SeedAsync(db);
            await NationalitySeeder.SeedAsync(uow);
        }
    }
}
