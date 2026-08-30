using Domain.Entities;

namespace Domain.Specification.AccessControl;

public sealed class CompanyRoleWithPermissionsSpecification : BaseSpecifications<CompanyRole>
{
    public CompanyRoleWithPermissionsSpecification(Guid companyId, Guid roleId)
        : base(x => x.CompanyId == companyId && x.Id == roleId)
    {
        AddInclude(x => x.Permissions);
    }
}
