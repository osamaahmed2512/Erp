using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.CreateSystemUser;

public sealed record CreateSystemUserCommand(Guid ActorId, CreateSystemUserDto Dto)
    : IRequest<BaseApiResponse<Guid>>;
