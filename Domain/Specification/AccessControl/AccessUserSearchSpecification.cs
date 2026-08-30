using Domain.Entities;
using Domain.Enum;

namespace Domain.Specification.AccessControl;

public sealed class AccessUserSearchSpecification : BaseSpecifications<ApplicationUser>
{
    public AccessUserSearchSpecification(string? search, AccountType accountType, Guid? companyId = null)
        : base(user =>
            user.AccountType == accountType &&
            (!companyId.HasValue || user.CompanyId == companyId) &&
            (string.IsNullOrWhiteSpace(search) ||
             (user.Email ?? string.Empty).ToLower().Contains(search) ||
             user.FirstName.ToLower().Contains(search) ||
             (user.LastName ?? string.Empty).ToLower().Contains(search)))
    {
        AddOrderBy(user => user.Email!);
        ApplyPagination(0, 100);
    }
}
