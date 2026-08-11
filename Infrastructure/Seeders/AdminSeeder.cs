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
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var email = "admin2512003@gmail.com";

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    FirstName= "admin",                  
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "123456");

                await userManager.AddToRoleAsync(user, SystemRoles.SuperAdmin.ToString());
            }
        }
    }
}
