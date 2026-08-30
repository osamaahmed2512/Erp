namespace Domain.Entities;

public sealed class SystemRole : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public bool IsProtected { get; set; }
    public ICollection<SystemRolePermission> Permissions { get; set; } = [];
    public ICollection<SystemUserRole> Users { get; set; } = [];
}
