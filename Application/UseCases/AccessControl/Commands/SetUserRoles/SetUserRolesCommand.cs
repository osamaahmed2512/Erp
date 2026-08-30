using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.SetUserRoles;

public sealed record SetUserRolesCommand(Guid ActorId, Guid CompanyId, Guid UserId, SetUserRolesDto Dto) : IRequest<BaseApiResponse>;
