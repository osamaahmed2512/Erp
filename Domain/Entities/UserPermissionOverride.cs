using Domain.Enum;

namespace Domain.Entities;

public sealed class UserPermissionOverride : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Guid PermissionDefinitionId { get; set; }
    public PermissionDefinition PermissionDefinition { get; set; } = null!;
    public PermissionEffect Effect { get; set; } = PermissionEffect.Deny;
}
