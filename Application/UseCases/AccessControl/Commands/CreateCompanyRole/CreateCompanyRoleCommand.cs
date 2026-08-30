using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.CreateCompanyRole;

public sealed record CreateCompanyRoleCommand(Guid ActorId, Guid CompanyId, CreateCompanyRoleDto Dto) : IRequest<BaseApiResponse<CompanyRoleDto>>;
