using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seeders
{
    public static class UserPermissionSeeder
    {
        public static async Task SeedAdminPermissionsAsync(
             UserManager<ApplicationUser> userManager)
        {
            var superAdmin = await userManager.FindByEmailAsync("admin2512003@gmail.com");

            if (superAdmin == null)
                return;

            var existingClaims = await userManager.GetClaimsAsync(superAdmin);

            foreach (var permission in Permissions.GetAll())
            {
                if (!existingClaims.Any(c =>
                    c.Type == "permission" &&
                    c.Value == permission))
                {
                    await userManager.AddClaimAsync(
                        superAdmin, new Claim("permission", permission));
                }
            }
        }
    }
}
