using Domain.Enum;


namespace Domain.Entities
{
    public class Contract:BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public string ContractNumber { get; set; }
        public decimal BasicSalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Draft;
        public ContractType Type { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public Employee Employee { get; set; }
    }
}
