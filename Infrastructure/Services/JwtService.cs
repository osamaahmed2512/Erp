using Application.Dtos.Auth;
using Application.Dtos.Token;
using Application.Interfaces.ExternalServices;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Employee;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

            return await CreateTokenAsync(appUser);
        }

        private async Task<TokenResponseDto> CreateTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

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

            var expires = DateTime.UtcNow.AddDays(_tokenConfig.Expiration);

            var token = new JwtSecurityToken(
                issuer: _tokenConfig.Issuer,
                audience: _tokenConfig.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponseDto
            {
                Token = jwt,
                ExpiresAt = expires
            };
        }
    }
}
