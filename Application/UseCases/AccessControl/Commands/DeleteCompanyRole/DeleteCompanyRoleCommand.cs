using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.DeleteCompanyRole;

public sealed record DeleteCompanyRoleCommand(Guid ActorId, Guid CompanyId, Guid RoleId) : IRequest<BaseApiResponse>;
