using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Params
{
    public class CompanyPaginationParams:PaginationParams
    {
        public Guid OwnerId { get; set; }
    }
}
