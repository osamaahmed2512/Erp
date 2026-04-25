using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PayrollRun
    {
        public Guid Id { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public DateTime RunDate { get; set; }

        public ICollection<Payslip> Payslips { get; set; }
    }
}
