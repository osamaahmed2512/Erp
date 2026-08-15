
using Domain.Enum;

namespace Domain.Entities
{
    public class Position:BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public EntityStatus Status { get; set; } = EntityStatus.Active;
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
    }
}
