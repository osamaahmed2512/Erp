using Application.Dtos.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetCompanyRoles;

public sealed record GetCompanyRolesQuery(Guid ActorId, Guid CompanyId) : IRequest<IReadOnlyList<CompanyRoleDto>>;
