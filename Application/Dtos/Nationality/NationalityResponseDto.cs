using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Nationality
{
    public class NationalityResponseDto
    {
        public Guid Id { get; set; } 
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string? IsoCode { get; set; }
        public string? IsoCode3 { get; set; }
    }
}
