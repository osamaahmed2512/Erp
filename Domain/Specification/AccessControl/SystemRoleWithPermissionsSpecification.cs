using Domain.Entities;

namespace Domain.Specification.AccessControl;

public sealed class SystemRoleWithPermissionsSpecification : BaseSpecifications<SystemRole>
{
    public SystemRoleWithPermissionsSpecification(Guid roleId)
        : base(x => x.Id == roleId)
    {
        AddInclude(x => x.Permissions);
    }
}
