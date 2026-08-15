using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum ContractStatus
    {
        Draft = 1,
        PendingApproval = 2,
        Active = 3,
        Expired = 4,
        Terminated = 5,
        Cancelled = 6
    }
}
