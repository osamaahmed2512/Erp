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
        public ContractStatus? Status { get; set; }
        public Guid? EmployeeId { get; set; }
    }
}
