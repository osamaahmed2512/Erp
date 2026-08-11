using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Params
{
    public class AttendancePaginationParams : PaginationParams
    {
        public Guid? EmployeeId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Search { get; set; }
    }
}
