using Application.Dtos.Response;
using Application.Interfaces.ExternalServices;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Application.UseCases.Auth.Commands.ResendConfirmEmail
{
    public class ResendConfirmEmailHandler : IRequestHandler<ResendConfirmEmailCommand, BaseApiResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        public ResendConfirmEmailHandler(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            this._emailService = emailService;
        }

        public async Task<BaseApiResponse> Handle(ResendConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return BaseApiResponse.Fail(404, "User not found");

            if (user.EmailConfirmed)
                return BaseApiResponse.Fail(400, "Already confirmed");

            if (user.EmailConfirmationExpiry > DateTime.UtcNow.AddMinutes(-2))
            {
                return BaseApiResponse.Fail(400, "Please wait before requesting another email");
            }

            user.EmailConfirmationExpiry = DateTime.UtcNow.AddHours(7);
            await _userManager.UpdateAsync(user);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var link = $"https://app.yourdomain.com/confirm-email?userId={user.Id}&token={encoded}";

            await _emailService.SendAsync(user.Email, "New confirmation link", $@"
                 <h2>Welcome</h2>
                 <p>Please confirm your email:</p>
                 <a href='{link}'>Confirm Email</a>
                 ");
            return new BaseApiResponse
            {
                StatusCode = 200,
                Message = "New confirmation link done successfully"
            };
        }
    }
}
