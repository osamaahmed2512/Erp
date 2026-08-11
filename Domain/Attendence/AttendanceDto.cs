using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Attendence
{
    public class AttendanceDto
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public double WorkedHours { get; set; }
        public double OvertimeHours { get; set; }
    }
}
