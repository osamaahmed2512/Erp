
using Domain.Enum;

namespace Domain.Entities
{
    public class Position:BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public EntityStatus Status { get; set; } = EntityStatus.Active;
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
