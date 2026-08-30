namespace Domain.Entities;

public sealed class SystemRolePermission : BaseEntity
{
    public Guid SystemRoleId { get; set; }
    public SystemRole SystemRole { get; set; } = null!;
    public Guid PermissionDefinitionId { get; set; }
    public PermissionDefinition PermissionDefinition { get; set; } = null!;
}
