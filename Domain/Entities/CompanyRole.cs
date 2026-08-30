namespace Domain.Entities;

public sealed class CompanyRole : BaseEntity
{
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public ICollection<CompanyRolePermission> Permissions { get; set; } = [];
    public ICollection<CompanyUserRole> Users { get; set; } = [];
}
