using Domain.Specification.Helper;
using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Attendance
{
    public class AttendanceSpecification : BaseSpecifications<Domain.Entities.Attendance>
    {
        // By Id
        public AttendanceSpecification(Guid id)
            : base(x => x.Id == id) { }

        // Open attendance check (CheckIn without CheckOut) per employee
        public AttendanceSpecification(Guid employeeId, bool openOnly)
            : base(x => x.EmployeeId == employeeId && x.CheckOut == null) { }

        // For Pagination
        public AttendanceSpecification(AttendancePaginationParams parms)
        {
            Expression<Func<Domain.Entities.Attendance, bool>> criteria = a => true;

            if (parms.EmployeeId.HasValue && parms.EmployeeId != Guid.Empty)
                criteria = criteria.AndAlso(a => a.EmployeeId == parms.EmployeeId.Value);

            if (parms.CompanyId.HasValue && parms.CompanyId != Guid.Empty)
                criteria = criteria.AndAlso(a => a.Employee.CompanyId == parms.CompanyId.Value);


            if (parms.From.HasValue)
                criteria = criteria.AndAlso(a => a.CheckIn >= parms.From.Value);

            if (parms.To.HasValue)
                criteria = criteria.AndAlso(a => a.CheckIn <= parms.To.Value);

            if (!string.IsNullOrWhiteSpace(parms.Search))
            {
                var s = parms.Search.Trim().ToLower();
                criteria = criteria.AndAlso(a =>
                    a.Employee.User.FirstName.ToLower().Contains(s) ||
                    (a.Employee.User.LastName != null && a.Employee.User.LastName.ToLower().Contains(s)));
            }

            AddCriteria(criteria);
            AddOrderByDescending(a => a.CheckIn);
        }
    }
}
