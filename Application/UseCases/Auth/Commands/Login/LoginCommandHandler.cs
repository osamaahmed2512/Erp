using Application.Dtos.Response;
using Application.Dtos.Token;
using Application.Interfaces.ExternalServices;
using MediatR;

namespace Application.UseCases.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseApiResponse<TokenResponseDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(
            IIdentityService identityService,
            IJwtService jwtService)
        {
            _identityService = identityService;
            _jwtService = jwtService;
        }

        public async Task<BaseApiResponse<TokenResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByEmailAsync(request.Email);

            if (user == null)
               return BaseApiResponse<TokenResponseDto>.Fail(400, "Invalid email or password");

                    if (!user.EmailConfirmed)
                return BaseApiResponse<TokenResponseDto>.Fail(403, "Please confirm your email first");
            var isPasswordValid = await _identityService.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
               return BaseApiResponse<TokenResponseDto>.Fail(400, "Invalid email or password");

            var token = await _jwtService.GenerateTokenAsync(user);

            return new BaseApiResponse<TokenResponseDto>
            {
                StatusCode = 200,
                Message = "Login successful",
                Data = token
            };
        }
    }
}