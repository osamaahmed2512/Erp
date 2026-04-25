using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Auth.Commands.ResendConfirmEmail
{
    public class ResendConfirmEmailCommand:IRequest<BaseApiResponse>
    {
        public string Email { get; set; }
    }
}
