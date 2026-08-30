using Application.Dtos.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetPermissionCatalog;

public sealed record GetPermissionCatalogQuery(Guid ActorId, Guid CompanyId) : IRequest<IReadOnlyList<SystemPageDto>>;
