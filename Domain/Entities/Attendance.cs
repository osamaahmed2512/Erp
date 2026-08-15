using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Attendance:BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateOnly Date { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int  LateMinutes { get; set; }
        public double? WorkedHours { get; set; }
        public int? EarlyLeaveMinutes { get; set; }
        public double OvertimeHours { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
