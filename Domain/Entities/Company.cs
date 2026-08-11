
using Domain.Enum;

namespace Domain.Entities
{
    public class Company:BaseEntity
    {
        public string Name { get; set; }
        public string? CompanyCode { get; set; }
        public string? Description { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? TaxNumber { get; set; }
        public string? CommercialRegistration { get; set; }
        public string? Website { get; set; }
        public EntityStatus Status { get; set; }= EntityStatus.Active;
        public Guid OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
