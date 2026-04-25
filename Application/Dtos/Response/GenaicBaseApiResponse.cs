using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Response
{
    public class BaseApiResponse<T> : BaseApiResponse
    {
        public T? Data { get; set; }
        public BaseApiResponse() { }

        public BaseApiResponse(int statusCode, string message, T? data = default)
            : base(statusCode, message)
        {
            this.Data = data;
        }
        public static BaseApiResponse<T> Success(int code, string message) =>
        new(code, message);

        public static BaseApiResponse<T> Fail(int code, string message) =>
            new(code, message);

    }
}
