using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Contract
{
    public class ContractResponse
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeFullName { get; set; } 
        public decimal BasicSalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } 
    }
}
