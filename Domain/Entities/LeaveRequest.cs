using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LeaveRequest
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public Guid LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public LeaveStatus Status { get; set; }

        public Guid? ApprovedById { get; set; }
        public Employee ApprovedBy { get; set; }
    }
}
