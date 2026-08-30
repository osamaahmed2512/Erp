using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.CreateSystemRole;

public sealed record CreateSystemRoleCommand(Guid ActorId, CreateSystemRoleDto Dto)
    : IRequest<BaseApiResponse<SystemRoleDto>>;
