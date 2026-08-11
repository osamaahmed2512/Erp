using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Nationality:BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string? IsoCode { get; set; }      
        public string? IsoCode3 { get; set; }     
        public bool IsActive { get; set; } = true;
        public ICollection<Employee> Employees { get; set; }
    }
}
