using Application.Dtos.AccessControl;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetUserAccess;

public sealed class GetUserAccessQueryHandler : IRequestHandler<GetUserAccessQuery, UserAccessDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetUserAccessQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public Task<UserAccessDto?> Handle(GetUserAccessQuery request, CancellationToken cancellationToken)
    {
        var spec = new BaseSpecifications<ApplicationUser>(x =>
            x.Id == request.UserId && x.AccountType == AccountType.Company && x.CompanyId == request.CompanyId);
        return _unitOfWork.Repository<ApplicationUser>().GetSingleProjectedAsync(x =>
            new UserAccessDto(x.Id,
                x.CompanyRoles.Select(role => role.CompanyRoleId).ToList(),
                x.PermissionOverrides.Where(item => item.Effect == PermissionEffect.Deny)
                    .Select(item => item.PermissionDefinitionId).ToList()),
            spec, cancellationToken);
    }
}
