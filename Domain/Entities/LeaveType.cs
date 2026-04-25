using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LeaveType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int DefaultDaysPerYear { get; set; }

        public ICollection<LeaveRequest> LeaveRequests { get; set; }
    }
}
