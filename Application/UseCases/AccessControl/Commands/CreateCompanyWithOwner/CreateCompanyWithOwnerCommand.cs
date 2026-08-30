using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.CreateCompanyWithOwner;

public sealed record CreateCompanyWithOwnerCommand(Guid ActorId, CreateCompanyWithOwnerDto Dto)
    : IRequest<BaseApiResponse<Guid>>;
