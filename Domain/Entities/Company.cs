
namespace Domain.Entities
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? Website { get; set; }
        public Guid OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
