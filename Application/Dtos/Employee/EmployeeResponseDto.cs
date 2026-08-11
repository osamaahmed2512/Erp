using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Employee
{
    public class EmployeeResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public string ComapnyName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string NationalityNumber { get; set; }
        public string Status { get; set; }
    }
}
