using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Departement
{
    public class DepartementPaginationSpecification: DepartmentSpecification
    {
        public DepartementPaginationSpecification(DepartmentPaginationParams paginationParams)
            :base(paginationParams)
        {
            ApplyPagination((paginationParams.PageIndex - 1) * paginationParams.PageSize, paginationParams.PageSize);
        }
    }
}
