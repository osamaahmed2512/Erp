using Application.Dtos.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetAccessUsers;

public sealed record GetAccessUsersQuery(Guid ActorId, Guid CompanyId, string? Search) : IRequest<IReadOnlyList<AccessUserDto>>;
