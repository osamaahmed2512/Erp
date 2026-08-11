using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Contract
{
    public class CreateContractDto
    {
        public Guid EmployeeId { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Basic salary must be greater than 0.")]
        public decimal BasicSalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
