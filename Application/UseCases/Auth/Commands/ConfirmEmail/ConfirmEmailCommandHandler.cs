using Application.Dtos.Response;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Auth.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler
        : IRequestHandler<ConfirmEmailCommand, BaseApiResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<BaseApiResponse<string>> Handle(
            ConfirmEmailCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                return BaseApiResponse<string>.Fail(404, "User not found");
            }

            if (user.EmailConfirmed)
            {
                return BaseApiResponse<string>.Fail(400, "Email already confirmed");
            }
            if (user.EmailConfirmationExpiry < DateTime.UtcNow)
            {
                return BaseApiResponse<string>.Fail(400, "Link expired. Please request a new confirmation email.");
            }

            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(request.Token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            return new BaseApiResponse<string>
            {
                StatusCode = 200,
                Message = "Email confirmed successfully"
            };
        }
    }
}
