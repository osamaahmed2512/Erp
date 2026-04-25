using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Response
{
    public class DevelopmentExceptionResponse:BaseApiResponse
    {
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
        public string? InnerStackTrace { get; set; }
        public string? Path { get; set; }
    }
}
