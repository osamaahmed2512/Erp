namespace Domain.Entities;

public sealed class CompanyUserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Guid CompanyRoleId { get; set; }
    public CompanyRole CompanyRole { get; set; } = null!;
}
