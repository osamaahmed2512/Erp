using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WorkingScheduleDay:BaseEntity
    {
        public Guid WorkingScheduleId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public bool IsWorkingDay { get; set; }
        public int BreakMinutes { get; set; }
        public WorkingSchedule WorkingSchedule { get; set; } = null!;
    }
}
