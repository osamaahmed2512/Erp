using Application.Dtos.Auth;
using Application.Dtos.Response;
using MediatR;


namespace Application.UseCases.Auth.Commands.Register
{
    public class RegisterUserCommand : IRequest<BaseApiResponse<string>>
    {
        public RegisterUserDto Dto { get; set; }

        public RegisterUserCommand(RegisterUserDto dto)
        {
            Dto = dto;
        }
    }
}
