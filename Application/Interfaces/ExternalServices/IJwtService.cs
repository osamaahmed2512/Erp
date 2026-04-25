using Application.Dtos.Auth;
using Application.Dtos.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ExternalServices
{
    public interface IJwtService
    {
        Task<TokenResponseDto> GenerateTokenAsync(AuthUserDto user);
    }
}
