using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.DeleteSystemRole;

public sealed record DeleteSystemRoleCommand(Guid ActorId, Guid RoleId) : IRequest<BaseApiResponse>;
