using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.SetSystemUserRoles;

public sealed record SetSystemUserRolesCommand(Guid ActorId, Guid UserId, SetUserRolesDto Dto)
    : IRequest<BaseApiResponse>;
