using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Attendence
{
    public class AttendanceSummaryDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int TotalDays { get; set; }
        public double TotalWorkedHours { get; set; }
        public double TotalOvertimeHours { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
