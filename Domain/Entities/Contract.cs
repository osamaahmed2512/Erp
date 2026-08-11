using Domain.Enum;


namespace Domain.Entities
{
    public class Contract:BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public decimal BasicSalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Active;
        public Employee Employee { get; set; }
    }
}
