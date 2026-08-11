using Domain.Specification.Helper;
using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Departement
{
    public class DepartmentSpecification : BaseSpecifications<Domain.Entities.Department>
    {

        public DepartmentSpecification(Guid id)
            : base(x => x.Id == id) { }

        public DepartmentSpecification(string name, Guid companyId)
            : base(x => x.Name.ToLower() == name.ToLower().Trim() && x.CompanyId == companyId) { }


        public DepartmentSpecification(string name, Guid companyId, Guid departmentId)
            : base(x => x.Name.ToLower() == name.ToLower().Trim()
                     && x.CompanyId == companyId
                     && x.Id != departmentId)
        { }

        public DepartmentSpecification(DepartmentPaginationParams paginationParams)
        {
            Expression<Func<Domain.Entities.Department, bool>> criteria = d => true;

            criteria = criteria.AndAlso(d => d.Status != Enum.EntityStatus.Deleted);

            if (!string.IsNullOrWhiteSpace(paginationParams.Search))
            {
                var s = paginationParams.Search.Trim().ToLower();
                criteria = criteria.AndAlso(d =>
                    d.Name.ToLower().Contains(s) ||
                    (d.Description != null && d.Description.ToLower().Contains(s))
                );
            }

            if (paginationParams.CompanyId.HasValue && paginationParams.CompanyId != Guid.Empty)
            {
                criteria = criteria.AndAlso(d => d.CompanyId == paginationParams.CompanyId.Value);
            }

            AddCriteria(criteria);
            AddOrderByDescending(d => d.CreatedAt);
        }
    }
}
