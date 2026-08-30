using Application.Dtos.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetSystemRoles;

public sealed record GetSystemRolesQuery(Guid ActorId) : IRequest<IReadOnlyList<SystemRoleDto>>;
