using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.SetSystemUserDenies;

public sealed record SetSystemUserDeniesCommand(Guid ActorId, Guid UserId, SetUserOverridesDto Dto)
    : IRequest<BaseApiResponse>;
