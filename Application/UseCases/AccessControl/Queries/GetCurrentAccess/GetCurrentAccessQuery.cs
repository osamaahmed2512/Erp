using Application.Dtos.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetCurrentAccess;

public sealed record GetCurrentAccessQuery(Guid UserId, Guid? CompanyId) : IRequest<CurrentAccessDto>;
