using Application.Dtos.Response;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Application.Interfaces.ExternalServices;
namespace Application.UseCases.Auth.Commands.Register
{

    public class RegisterUserCommandHandler
        : IRequestHandler<RegisterUserCommand, BaseApiResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<BaseApiResponse<string>> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BaseApiResponse<string>.Fail(400, "Email already exists");
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName 

            };
         
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BaseApiResponse<string>.Fail(400, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            user.EmailConfirmationExpiry = DateTime.UtcNow.AddHours(7);
            await _userManager.UpdateAsync(user);
            await _userManager.AddToRoleAsync(user, SystemRoles.Owner.ToString());

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

            var confirmUrl = $"https://app.yourdomain.com/confirm-email?userId={user.Id}&token={encodedToken}";

            await _emailService.SendAsync(
                user.Email,
                "Confirm your email",
                $@"
                 <h2>Welcome</h2>
                 <p>Please confirm your email:</p>
                 <a href='{confirmUrl}'>Confirm Email</a>
                 "
                );
                               
            return new BaseApiResponse<string>
            {
                StatusCode = 200,
                Message = "User created successfully. Please check your email to confirm your account."
            };
        }
    }
}
