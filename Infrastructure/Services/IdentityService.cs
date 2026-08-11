using Application.Dtos.Auth;
using Application.Interfaces.ExternalServices;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<AuthUserDto?> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return null;

            return new AuthUserDto
            {
                Id = user.Id,
                Email = user.Email,
                EmailConfirmed=user.EmailConfirmed
            };
        }

        public async Task<bool> CheckPasswordAsync(AuthUserDto user, string password)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            return await _userManager.CheckPasswordAsync(appUser, password);
        }
        public async Task<Guid> CreateUserAsync(string email, string password ,string Role, string firstName, string lastName,string? phone)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName=firstName,
                LastName=lastName,
                EmailConfirmed=true
            };
            if(phone != null)
            {
                user.PhoneNumber = phone;
            }
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception("User creation failed");

            await _userManager.AddToRoleAsync(user, Role);
            return user.Id;
        }
        public async Task ChangeEmailAsync(Guid userId, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new Exception("User not found.");

            user.Email = newEmail;
            user.UserName = newEmail;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        public async Task ChangePasswordAsync(Guid userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new Exception("User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        public async Task<AuthUserDto?> FindByPhoneAsync(string phone)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phone);

            if (user == null)
                return null;

            return new AuthUserDto
            {
                Id = user.Id,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed
            };
        }
    }
}
