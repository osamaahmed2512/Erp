using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Employee
{
    public class EmployeeCountSpecification:EmployeeSpecification
    {
        public EmployeeCountSpecification(EmployeePaginationParams paginationParams)
            :base(paginationParams)
        {

        }
    }
}
