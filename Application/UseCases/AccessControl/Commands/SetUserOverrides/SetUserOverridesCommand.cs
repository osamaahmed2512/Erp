using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.SetUserOverrides;

public sealed record SetUserOverridesCommand(Guid ActorId, Guid CompanyId, Guid UserId, SetUserOverridesDto Dto) : IRequest<BaseApiResponse>;
