

namespace Application.Dtos.Contract
{
    public class ContractDto
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; }
        public decimal BasicSalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
    }
}
