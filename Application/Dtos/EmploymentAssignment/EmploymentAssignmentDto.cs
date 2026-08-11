namespace Application.Dtos.EmploymentAssignment
{
    public class EmploymentAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public Guid PositionId { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public Guid? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
    }
}
