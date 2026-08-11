

namespace Application.Dtos.Company
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? Website { get; set; }
        public string Status { get; set; }
        public string OwnerName { get; set; }
        public int TotalEmployees { get; set; }
    }
}
