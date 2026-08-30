using Application.Dtos.Auth;
using Application.Interfaces.ExternalServices;
using Domain.Enum;

namespace Application.Tests.Fakes;

internal sealed class FakeIdentityService : IIdentityService
{
    public Guid CreatedUserId { get; } = Guid.NewGuid();
    public Guid? AssignedCompanyId { get; private set; }

    public Task<AuthUserDto?> FindByEmailAsync(string email) => Task.FromResult<AuthUserDto?>(null);
    public Task<AuthUserDto?> FindByPhoneAsync(string phone) => Task.FromResult<AuthUserDto?>(null);
    public Task<bool> CheckPasswordAsync(AuthUserDto user, string password) => Task.FromResult(true);

    public Task<Guid> CreateUserAsync(string email, string password, string role, string firstName,
        string lastName, string? phone = null, AccountType accountType = AccountType.System,
        Guid? companyId = null) => Task.FromResult(CreatedUserId);

    public Task AssignCompanyAsync(Guid userId, Guid companyId)
    {
        AssignedCompanyId = companyId;
        return Task.CompletedTask;
    }

    public Task ChangeEmailAsync(Guid userId, string newEmail) => Task.CompletedTask;
    public Task ChangePasswordAsync(Guid userId, string newPassword) => Task.CompletedTask;
}
