
using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Employee:BaseEntity
    {
        public Guid UserId { get; set; }
        [MaxLength(20)]
        public string EmpCode { get; set; }
        [MaxLength(14)]
        public string NationalityNumber { get; set; }
        public DateOnly BirthDate { get; set; }
        [MaxLength(1)]
        public Gender Gender { get; set; }
        public Guid CompanyId { get; set; }
        [MaxLength(260)]
        public string? ProfilePhoto { get; set; }

        public bool IsDeleted { get; set; } = false;
        [MaxLength(2)]
        public EmployeeStatus Status { get; set; }
     
        [MaxLength(500)]
        public string Address { get; set; }
        [MaxLength(1)]
        public MaritalStatus MartielStatus { get; set; }
        public Guid NationalityId { get; set; }
        public Nationality Nationality { get; set; }
        public ApplicationUser User { get; set; }
        public Company Company { get; set; }
        public ICollection<Contract> Contracts { get; set; }
        public ICollection<EmploymentAssignment> EmploymentAssignments { get; set; } = [];

    }
}
