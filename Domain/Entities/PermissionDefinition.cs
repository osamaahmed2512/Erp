namespace Domain.Entities;

public sealed class PermissionDefinition : BaseEntity
{
    public Guid SystemPageId { get; set; }
    public SystemPage SystemPage { get; set; } = null!;
    public string Key { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
