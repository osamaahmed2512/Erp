

using Application.Dtos.Auth;
using Domain.Enum;

namespace Application.Interfaces.ExternalServices
{
    public interface IIdentityService
    {
        Task<AuthUserDto?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(AuthUserDto user, string password);
        Task<Guid> CreateUserAsync(string email, string password, string role, string firstName,
            string lastName, string? phone = null, AccountType accountType = AccountType.System,
            Guid? companyId = null);
        Task AssignCompanyAsync(Guid userId, Guid companyId);
        Task ChangeEmailAsync(Guid userId, string newEmail);
        Task ChangePasswordAsync(Guid userId, string newPassword);
        Task<AuthUserDto?> FindByPhoneAsync(string phone);

    }
}
