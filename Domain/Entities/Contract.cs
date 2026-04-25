using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Contract
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public decimal BasicSalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ContractStatus Status { get; set; }
        public Employee Employee { get; set; }
    }
}
