using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum AttendanceStatus
    {
        Present = 1,
        Absent = 2,
        Late = 3,
        OnLeave = 4,
        Holiday = 5,
        DayOff = 6
    }
}
