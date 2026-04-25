using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Response
{
    public class ValidationErrorResponse:BaseApiResponse
    {
        public Dictionary<string, IEnumerable<string>> Errors { get; set; } = new();
        public ValidationErrorResponse(int statusCode, string message, Dictionary<string, IEnumerable<string>> errors) : base(400, message)
        {
            Errors = errors;
        }
    }
}
