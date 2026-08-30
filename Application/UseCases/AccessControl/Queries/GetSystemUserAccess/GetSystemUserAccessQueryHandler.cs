using Application.Dtos.AccessControl;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetSystemUserAccess;

public sealed class GetSystemUserAccessQueryHandler : IRequestHandler<GetSystemUserAccessQuery, UserAccessDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetSystemUserAccessQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public Task<UserAccessDto?> Handle(GetSystemUserAccessQuery request, CancellationToken cancellationToken)
    {
        var spec = new BaseSpecifications<ApplicationUser>(x =>
            x.Id == request.UserId && x.AccountType == AccountType.System);
        return _unitOfWork.Repository<ApplicationUser>().GetSingleProjectedAsync(x =>
            new UserAccessDto(x.Id,
                x.SystemRoles.Select(role => role.SystemRoleId).ToList(),
                x.PermissionOverrides.Where(item => item.Effect == PermissionEffect.Deny)
                    .Select(item => item.PermissionDefinitionId).ToList()),
            spec, cancellationToken);
    }
}
