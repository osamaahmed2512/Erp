using Domain.Specification.Helper;
using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Departement
{
    public class DepartmentCountSpecification : DepartmentSpecification
    {
        public DepartmentCountSpecification(DepartmentPaginationParams paginationParams)
            :base(paginationParams)
        {

        }
    }
}
