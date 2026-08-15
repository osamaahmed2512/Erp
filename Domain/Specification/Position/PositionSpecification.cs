using Domain.Specification.Helper;
using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Position
{
    public class PositionSpecification : BaseSpecifications<Domain.Entities.Position>
    {
        public PositionSpecification(Guid id)
            : base(x => x.Id == id) { }

        public PositionSpecification(string title, Guid departmentId)
            : base(x => x.Title.ToLower() == title.ToLower().Trim() && x.DepartmentId == departmentId) { }

        public PositionSpecification(string title, Guid departmentId, Guid excludeId)
            : base(x => x.Title.ToLower() == title.ToLower().Trim()
                     && x.DepartmentId == departmentId
                     && x.Id != excludeId)
        { }

        public PositionSpecification(PositionPaginationParams parms)
        {
            Expression<Func<Domain.Entities.Position, bool>> criteria = p => true;
            criteria = criteria.AndAlso(p => p.Status != Enum.EntityStatus.Deleted);

            if (!string.IsNullOrWhiteSpace(parms.Search))
            {
                var s = parms.Search.Trim().ToLower();
                criteria = criteria.AndAlso(p =>
                    p.Title.ToLower().Contains(s) ||
                    (p.Description != null && p.Description.ToLower().Contains(s)));
            }

            if (parms.CompanyId.HasValue && parms.CompanyId != Guid.Empty)
                criteria = criteria.AndAlso(p => p.Department.CompanyId == parms.CompanyId.Value);

            if (parms.DepartmentId.HasValue && parms.DepartmentId != Guid.Empty)
                criteria = criteria.AndAlso(p => p.DepartmentId == parms.DepartmentId.Value);

            AddCriteria(criteria);
            AddOrderByDescending(p => p.CreatedAt);
        }
    }
}
