using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WorkingSchedule:BaseEntity
    {
        public string Name { get; set; }
        public string CompanyId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
        public ICollection<WorkingScheduleDay> Days { get; set; }
    = new List<WorkingScheduleDay>();
    }
}
