using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.EmploymentAssignment
{
    public class CreateEmploymentAssignmentDto
    {
        public Guid DepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public Guid? ManagerId { get; set; }

        [Required]
        public DateOnly EffectiveFrom { get; set; }
    }
}
