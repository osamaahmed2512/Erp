namespace Domain.Entities;

public sealed class CompanyRolePermission : BaseEntity
{
    public Guid CompanyRoleId { get; set; }
    public CompanyRole CompanyRole { get; set; } = null!;
    public Guid PermissionDefinitionId { get; set; }
    public PermissionDefinition PermissionDefinition { get; set; } = null!;
}
