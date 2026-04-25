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
        public EmployeeSpecification(PaginationParams paginationParams)
        {
            Expression<Func<Domain.Entities.Employee, bool>> criteria = c => true;
            if (!string.IsNullOrWhiteSpace(paginationParams.Search))
            {
                var s = paginationParams.Search.Trim().ToLowerInvariant();
                criteria = criteria.AndAlso(
                  e => e.User.FirstName.Contains(s) ||
                       (e.User.LastName !=null|| e.User.LastName.Contains(s))
                       
              );
            }
            AddCriteria(criteria);

            AddOrderByDescending(e => e.CreatedAt);
        }
    }
}
