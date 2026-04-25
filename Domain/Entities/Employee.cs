using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Employee:BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; }
        public ApplicationUser User { get; set; }
        public Company Company { get; set; }
        public ICollection<Contract> Contracts { get; set; }

    }
}
