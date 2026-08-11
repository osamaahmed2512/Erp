using Domain.Specification.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Attendance
{
    public class AttendanceCountSpecification: AttendanceSpecification
    {
        public AttendanceCountSpecification(AttendancePaginationParams paginationParams)
            :base(paginationParams)
        {
            
        }
    }
}
