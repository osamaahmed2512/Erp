using Domain.Entities;
using Domain.Interfaces.UnitOfWork;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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

            await RoleSeeder.SeedAsync(roleManager);
            await AdminSeeder.SeedAsync(userManager);
            await UserPermissionSeeder.SeedAdminPermissionsAsync(userManager);
            await NationalitySeeder.SeedAsync(uow);
        }
    }
}
