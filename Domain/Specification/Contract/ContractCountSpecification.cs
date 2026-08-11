using Domain.Enum;
using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Contract
{
    public class ContractCountSpecification : ContractSpecification
    {
        public ContractCountSpecification(ContractPaginationParams parms)
            :base(parms)
        {
        }
    }
}
