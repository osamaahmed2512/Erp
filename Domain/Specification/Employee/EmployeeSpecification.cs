using Domain.Entities;
using Domain.Specification.Helper;
using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Employee
{
    public class EmployeeSpecification:BaseSpecifications<Domain.Entities.Employee>
    {
        public EmployeeSpecification(Guid Id) : base(e => e.Id == Id)
        {
            AddInclude(e =>e.User);
        }

        public EmployeeSpecification(string PhoneNumber,string Email) 
            : base(e => e.User.PhoneNumber==PhoneNumber ||e.User.Email==Email)
        {
            AddInclude(e => e.User);
        }
        public EmployeeSpecification(EmployeePaginationParams paginationParams)
        {
            Expression<Func<Domain.Entities.Employee, bool>> criteria = c => true;
            if (!string.IsNullOrWhiteSpace(paginationParams.Search))
            {
                var s = paginationParams.Search.Trim().ToLower();
                criteria = criteria.AndAlso(
                    e => e.User.FirstName.ToLower().Contains(s) ||
                         (e.User.LastName != null && e.User.LastName.ToLower().Contains(s)) ||
                         (e.User.Email != null && e.User.Email.ToLower().Contains(s)) ||
                         (e.User.PhoneNumber != null && e.User.PhoneNumber.Contains(s)) ||
                         e.Company.Name.ToLower().Contains(s));
            }
            if (paginationParams.CompanyId.HasValue && paginationParams.CompanyId.Value != Guid.Empty)
                criteria = criteria.AndAlso(e => e.CompanyId == paginationParams.CompanyId.Value);
            AddCriteria(criteria);

            AddOrderByDescending(e => e.CreatedAt);
        }
    }
}
