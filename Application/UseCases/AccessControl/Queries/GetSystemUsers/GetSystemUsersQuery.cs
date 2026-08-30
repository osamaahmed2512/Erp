using Application.Dtos.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetSystemUsers;

public sealed record GetSystemUsersQuery(Guid ActorId, string? Search)
    : IRequest<IReadOnlyList<AccessUserDto>>;
