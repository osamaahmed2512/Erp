using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Auth.Commands.ConfirmEmail
{
    public class ConfirmEmailCommand: IRequest<BaseApiResponse<string>>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
