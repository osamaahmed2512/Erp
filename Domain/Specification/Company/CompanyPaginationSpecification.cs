using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Company
{
    public class CompanyPaginationSpecification:CompanySpecification
    {
        public CompanyPaginationSpecification(CompanyPaginationParams paginationParams)
            :base(paginationParams)
        {
            ApplyPagination((paginationParams.PageIndex - 1) * paginationParams.PageSize, paginationParams.PageSize);
        }
    }
}
