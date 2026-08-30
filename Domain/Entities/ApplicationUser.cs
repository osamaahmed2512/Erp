using Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? ImgUrl { get; set; }
    public DateTime? EmailConfirmationExpiry { get; set; }
    public ICollection<Company> OwnedCompanies { get; set; } = [];
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime SessionExpiryTime { get; set; }
    public bool IsRootSuperAdmin { get; set; }
    public AccountType AccountType { get; set; } = AccountType.System;
    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }
    public ICollection<CompanyUserRole> CompanyRoles { get; set; } = [];
    public ICollection<SystemUserRole> SystemRoles { get; set; } = [];
    public ICollection<UserPermissionOverride> PermissionOverrides { get; set; } = [];
    public bool IsExpired => DateTime.UtcNow >= RefreshTokenExpiryTime;
    public bool SessionExpired => DateTime.UtcNow > SessionExpiryTime;
}
