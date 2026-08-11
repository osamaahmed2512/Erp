using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Position
{
    public class PositionCountSpecification:PositionSpecification
    {
        public PositionCountSpecification(PositionPaginationParams parms)
            :base(parms)
        {
            
        }
    }
}
