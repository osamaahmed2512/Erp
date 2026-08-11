

using Application.Dtos.Auth;

namespace Application.Interfaces.ExternalServices
{
    public interface IIdentityService
    {
        Task<AuthUserDto?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(AuthUserDto user, string password);
        Task<Guid> CreateUserAsync(string email, string password, string Role ,string firstName ,string lastName, string? phone=null);
        Task ChangeEmailAsync(Guid userId, string newEmail);
        Task ChangePasswordAsync(Guid userId, string newPassword);
        Task<AuthUserDto?> FindByPhoneAsync(string phone);

    }
}
