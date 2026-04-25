using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SalaryComponent
    {
        public Guid Id { get; set; }

        public string Name { get; set; } // Allowance / Deduction
        public decimal Amount { get; set; }

        public SalaryComponentType Type { get; set; }

        public Guid PayslipId { get; set; }
        public Payslip Payslip { get; set; }
    }
}
