using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Department:BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public EntityStatus Status { get; set; } = EntityStatus.Active;
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
