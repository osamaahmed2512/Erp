using Domain.Enum;

namespace Application.Dtos.AccessControl;

public sealed record AccessCompanyDto(Guid Id, string Name);
public sealed record PermissionDto(Guid Id, string Key, string Action);
public sealed record SystemPageDto(
    Guid Id, string Key, string Name, string Module, string Category,
    string Route, string Icon, int DisplayOrder, PageAudience Audience,
    IReadOnlyList<PermissionDto> Permissions);
public sealed record CompanyModuleDto(string Name, IReadOnlyList<SystemPageDto> Pages);
public sealed record CurrentAccessDto(
    AccountType AccountType, bool IsRootSuperAdmin, bool IsCompanyOwner, Guid? CompanyId,
    IReadOnlyList<AccessCompanyDto> Companies,
    IReadOnlyList<string> PermissionKeys,
    IReadOnlyList<SystemPageDto> Pages);
public sealed record CompanyRoleDto(Guid Id, string Name, bool IsSystem, IReadOnlyList<Guid> PermissionIds);
public sealed record SystemRoleDto(Guid Id, string Name, bool IsProtected, IReadOnlyList<Guid> PermissionIds);
public sealed record AccessUserDto(
    Guid Id, string Email, string Name, AccountType AccountType,
    bool IsRootSuperAdmin, bool IsCompanyOwner);
public sealed record PermissionOverrideDto(Guid PermissionId, PermissionEffect Effect);
public sealed record UserAccessDto(Guid UserId, IReadOnlyList<Guid> RoleIds, IReadOnlyList<Guid> DeniedPermissionIds);

public sealed record CreateCompanyRoleDto(string Name, IReadOnlyList<Guid> PermissionIds);
public sealed record UpdateCompanyRoleDto(string Name, IReadOnlyList<Guid> PermissionIds);
public sealed record CreateSystemRoleDto(string Name, IReadOnlyList<Guid> PermissionIds);
public sealed record UpdateSystemRoleDto(string Name, IReadOnlyList<Guid> PermissionIds);
public sealed record SetUserRolesDto(IReadOnlyList<Guid> RoleIds);
public sealed record SetUserOverridesDto(IReadOnlyList<Guid> DeniedPermissionIds);
public sealed record CreateSystemUserDto(
    string Email, string Password, string FirstName, string LastName, string? Phone);
public sealed record CreateCompanyWithOwnerDto(
    string Name, string Email, string Phone, string? Description, string? Country,
    string? City, string? Address, string? PostalCode, string? TaxNumber,
    string? CommercialRegistration, string? Website,
    string OwnerEmail, string OwnerPassword, string OwnerFirstName,
    string OwnerLastName, string? OwnerPhone,
    IReadOnlyList<string> EnabledModules);
