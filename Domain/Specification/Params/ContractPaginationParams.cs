using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Params
{
    public class ContractPaginationParams : PaginationParams
    {
        public Guid? EmployeeId { get; set; }
        public Guid? CompanyId { get; set; }
        public ContractStatus? Status { get; set; }
        public string? Search { get; set; }
    }
}
