using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Departement
{
    public class CreateDepartmentDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
