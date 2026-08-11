using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Employee
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string NationalityNumber { get; set; }
        public Guid nationalityId { get; set; }
        public string Address { get; set; }
        public EmployeeStatus Status { get; set; }
        public MaritalStatus MartielStatus { get; set; }
        public DateOnly BirthDate { get; set; }
        public Gender Gender { get; set; }
        public Guid companyId { get; set; }
    }
}
