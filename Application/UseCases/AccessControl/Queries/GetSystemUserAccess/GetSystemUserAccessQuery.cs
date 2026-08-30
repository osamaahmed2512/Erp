using Application.Dtos.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetSystemUserAccess;

public sealed record GetSystemUserAccessQuery(Guid ActorId, Guid UserId) : IRequest<UserAccessDto?>;
