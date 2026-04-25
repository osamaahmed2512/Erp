using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Response
{
    public class BaseApiResponse
    {
        public int StatusCode { get; set; } = 200;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public BaseApiResponse() { }
        public BaseApiResponse(int statusCode, string message)
        {
            this.StatusCode = statusCode;
            this.Message = message;
        }

        public static BaseApiResponse Success(int code, string message) =>
                 new(code, message);

        public static BaseApiResponse Fail(int code, string message) =>
            new(code, message);
    }
}
