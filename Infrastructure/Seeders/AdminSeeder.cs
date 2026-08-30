using Domain.Entities;
using Domain.Enum;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seeders
{
    public class AdminSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, string rootEmail)
        {
            var email = rootEmail;

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    FirstName= "admin",                  
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    AccountType = AccountType.System
                };

                await userManager.CreateAsync(user, "123456");

                await userManager.AddToRoleAsync(user, SystemRoles.SuperAdmin.ToString());
            }
            if (!user.IsRootSuperAdmin)
            {
                user.IsRootSuperAdmin = true;
                await userManager.UpdateAsync(user);
            }
            if (user.AccountType != AccountType.System || user.CompanyId.HasValue)
            {
                user.AccountType = AccountType.System;
                user.CompanyId = null;
                await userManager.UpdateAsync(user);
            }
        }
    }
}
