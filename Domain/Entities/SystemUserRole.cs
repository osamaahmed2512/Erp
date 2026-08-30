namespace Domain.Entities;

public sealed class SystemUserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Guid SystemRoleId { get; set; }
    public SystemRole SystemRole { get; set; } = null!;
}
