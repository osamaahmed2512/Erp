using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Position
{
    public class PositionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public int TotalEmployees { get; set; }
    }
}
