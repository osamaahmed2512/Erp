namespace Domain.Entities
{
    public class EmploymentAssignment : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public Guid PositionId { get; set; }
        public Position Position { get; set; } = null!;

        public Guid? ManagerId { get; set; }
        public Employee? Manager { get; set; }

        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
    }
}
