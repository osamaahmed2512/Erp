using Application.Dtos.Auth;
using Application.Dtos.Token;
using Application.Interfaces.ExternalServices;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Employee;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class JwtService:IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenConfig _tokenConfig;
        private readonly IUnitOfWork _unitOfWork;
        public JwtService(
            UserManager<ApplicationUser> userManager,
            IOptions<TokenConfig> tokenOptions,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _tokenConfig = tokenOptions.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<TokenResponseDto> GenerateTokenAsync(AuthUserDto user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (appUser == null)
                throw new Exception("User not found");
            appUser.SessionExpiryTime = DateTime.UtcNow.AddDays(_tokenConfig.SessionExpiryTime);
            return await CreateTokenAsync(appUser);
        }
        public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
                throw new Exception("Invalid token");

            var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                throw new Exception("User not found");

            // Check refresh token
            if (user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new Exception("Invalid or expired refresh token");
            }
            if (user.SessionExpired)
            {
                throw new Exception("Session expired");
            }
            // Generate new tokens (ROTATION)
            return await CreateTokenAsync(user);
        }

        private async Task<TokenResponseDto> CreateTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _userManager.GetClaimsAsync(user);
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? "")
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            foreach (var permission in permissions)
            {
                if (permission.Type == "permission")
                {
                    claims.Add(new Claim("permission", permission.Value));
                }
            }
            if (roles.Contains(SystemRoles.Employee.ToString()))
            {
                var spec = new EmployeeSpecification(user.Id);

                var employee = await _unitOfWork.Repository<Employee>()
                    .GetSingleProjectedAsync(x => new
                    {
                        x.Id
                    }, spec);

                if (employee != null)
                {
                    claims.Add(new Claim("employeeId", employee.Id.ToString()));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_tokenConfig.Expiration);

            var token = new JwtSecurityToken(
                issuer: _tokenConfig.Issuer,
                audience: _tokenConfig.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);


            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_tokenConfig.RefreshTokenExpirationDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiry;


            await _userManager.UpdateAsync(user);
            return new TokenResponseDto
            {
                Token = jwt,
                ExpiresAt = expires,
                RefreshToken = refreshToken,
                ExpiresRefreshTokenAt = refreshTokenExpiry,
                SessionExpiryTime = DateTime.SpecifyKind(user.SessionExpiryTime, DateTimeKind.Utc),
                Role = roles.FirstOrDefault()
            };
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                // Only the signature matters when extracting claims from an already-expired
                // token; issuer/audience/lifetime were validated when it was first issued.
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_tokenConfig.Key)),
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
