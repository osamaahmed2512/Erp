using Domain.Enum;

namespace Domain.Entities;

public sealed class SystemPage : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string Icon { get; set; } = "circle";
    public int DisplayOrder { get; set; }
    public PageAudience Audience { get; set; } = PageAudience.Both;
    public bool IsActive { get; set; } = true;
    public ICollection<PermissionDefinition> Permissions { get; set; } = [];
}
