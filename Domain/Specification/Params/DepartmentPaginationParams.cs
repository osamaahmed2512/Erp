using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Params
{
    public class DepartmentPaginationParams : PaginationParams
    {
        public string? Search { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
