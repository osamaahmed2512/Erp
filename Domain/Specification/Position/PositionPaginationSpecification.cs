using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Position
{
    public class PositionPaginationSpecification: PositionSpecification
    {
        public PositionPaginationSpecification(PositionPaginationParams parms)
            :base(parms)
        {
            ApplyPagination((parms.PageIndex - 1) * parms.PageSize, parms.PageSize);
        }
    }
}
