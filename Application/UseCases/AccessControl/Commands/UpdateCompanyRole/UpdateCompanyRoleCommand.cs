using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.UpdateCompanyRole;

public sealed record UpdateCompanyRoleCommand(Guid ActorId, Guid CompanyId, Guid RoleId, UpdateCompanyRoleDto Dto) : IRequest<BaseApiResponse>;
