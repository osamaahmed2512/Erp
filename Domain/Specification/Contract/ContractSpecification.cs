using Domain.Enum;
using Domain.Specification.Helper;
using Domain.Specification.Params;
using System.Linq.Expressions;


namespace Domain.Specification.Contract
{
    public class ContractSpecification : BaseSpecifications<Domain.Entities.Contract>
    {
        // By Id
        public ContractSpecification(Guid id)
            : base(x => x.Id == id) { }

        // Active contract check per employee (one Active at a time)
        public ContractSpecification(Guid employeeId, ContractStatus status)
            : base(x => x.EmployeeId == employeeId && x.Status == status) { }

        // Active contract excluding current (update validation)
        public ContractSpecification(Guid employeeId, ContractStatus status, Guid excludeId)
            : base(x => x.EmployeeId == employeeId && x.Status == status && x.Id != excludeId) { }

        // For Pagination
        public ContractSpecification(ContractPaginationParams parms)
        {
            Expression<Func<Domain.Entities.Contract, bool>> criteria = c => true;
            // Default: exclude Terminated (soft-deleted)
            criteria = criteria.AndAlso(c => c.Status != ContractStatus.Terminated);

            if (parms.EmployeeId.HasValue && parms.EmployeeId != Guid.Empty)
                criteria = criteria.AndAlso(c => c.EmployeeId == parms.EmployeeId.Value);

            if (parms.CompanyId.HasValue && parms.CompanyId != Guid.Empty)
                criteria = criteria.AndAlso(c => c.Employee.CompanyId == parms.CompanyId.Value);

            if (!string.IsNullOrWhiteSpace(parms.Status.ToString()))
            {
               
                criteria = criteria.AndAlso(c => c.Status ==parms.Status );
            }
                

            if (!string.IsNullOrWhiteSpace(parms.Search))
            {
                var s = parms.Search.Trim().ToLower();
                criteria = criteria.AndAlso(c =>
                    c.Employee.User.FirstName.ToLower().Contains(s) ||
                    (c.Employee.User.LastName != null && c.Employee.User.LastName.ToLower().Contains(s)));
            }

            AddCriteria(criteria);
            AddOrderByDescending(c => c.CreatedAt);
        }
    }
}
