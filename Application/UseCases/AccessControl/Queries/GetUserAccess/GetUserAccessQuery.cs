using Application.Dtos.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetUserAccess;

public sealed record GetUserAccessQuery(Guid ActorId, Guid CompanyId, Guid UserId) : IRequest<UserAccessDto?>;
