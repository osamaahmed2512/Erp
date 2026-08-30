using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.UpdateSystemRole;

public sealed record UpdateSystemRoleCommand(Guid ActorId, Guid RoleId, UpdateSystemRoleDto Dto)
    : IRequest<BaseApiResponse>;
