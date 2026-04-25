    using Application.Dtos.Response;
using Application.Dtos.Token;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Auth.Commands.Login
{
    public class LoginCommand : IRequest<BaseApiResponse<TokenResponseDto>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
