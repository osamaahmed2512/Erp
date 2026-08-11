using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Position
{
    public class CreatePositionDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid CompanyId { get; set; }
    }
}
