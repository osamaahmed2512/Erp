using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum EmployeeStatus
    {   
        Created=0,
        Active = 1,
        OnLeave = 2,
        Suspended = 3,
        Resigned = 4,
        Terminated = 5,
        Retired = 6
    }
}
