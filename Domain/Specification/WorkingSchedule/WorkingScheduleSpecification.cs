using Domain.Specification.Helper;
using Domain.Specification.Params;
using System.Linq.Expressions;

namespace Domain.Specification.WorkingSchedule;

public class WorkingScheduleSpecification : BaseSpecifications<Entities.WorkingSchedule>
{
    public WorkingScheduleSpecification(Guid id, bool includeDetails = false) : base(s => s.Id == id)
    {
        if (includeDetails)
        {
            AddInclude(s => s.Company);
            AddInclude(s => s.Days);
        }
    }

    public WorkingScheduleSpecification(string name, Guid companyId, Guid? excludedId = null)
        : base(s => s.CompanyId == companyId && s.Name.ToLower() == name.ToLower().Trim()
                    && (!excludedId.HasValue || s.Id != excludedId.Value)) { }

    public WorkingScheduleSpecification(WorkingSchedulePaginationParams parameters, Guid ownerId, bool isSuperAdmin)
    {
        Expression<Func<Entities.WorkingSchedule, bool>> criteria = s => isSuperAdmin || s.Company.OwnerId == ownerId;
        if (parameters.CompanyId.HasValue && parameters.CompanyId.Value != Guid.Empty)
            criteria = criteria.AndAlso(s => s.CompanyId == parameters.CompanyId.Value);
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim().ToLower();
            criteria = criteria.AndAlso(s => s.Name.ToLower().Contains(search) || s.Company.Name.ToLower().Contains(search));
        }
        AddCriteria(criteria);
        AddOrderByDescending(s => s.CreatedAt);
    }
}

public class WorkingSchedulePaginationSpecification : WorkingScheduleSpecification
{
    public WorkingSchedulePaginationSpecification(WorkingSchedulePaginationParams parameters, Guid ownerId, bool isSuperAdmin)
        : base(parameters, ownerId, isSuperAdmin) =>
        ApplyPagination((parameters.PageIndex - 1) * parameters.PageSize, parameters.PageSize);
}
