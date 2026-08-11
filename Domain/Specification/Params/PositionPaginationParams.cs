using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Params
{
    public class PositionPaginationParams : PaginationParams
    {
        public Guid? CompanyId { get; set; }
        public string? Search { get; set; }
    }
}
