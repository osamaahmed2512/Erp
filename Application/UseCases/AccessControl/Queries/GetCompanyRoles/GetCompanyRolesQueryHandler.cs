using Application.Dtos.AccessControl;
using Domain.Entities;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetCompanyRoles;

public sealed class GetCompanyRolesQueryHandler : IRequestHandler<GetCompanyRolesQuery, IReadOnlyList<CompanyRoleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCompanyRolesQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<CompanyRoleDto>> Handle(GetCompanyRolesQuery request, CancellationToken cancellationToken)
    {
        var roleSpec = new BaseSpecifications<CompanyRole>(x => x.CompanyId == request.CompanyId);
        return (await _unitOfWork.Repository<CompanyRole>().GetProjectedAsync(x =>
                new CompanyRoleDto(x.Id, x.Name, x.IsSystem,
                    x.Permissions.Select(permission => permission.PermissionDefinitionId).ToList()), roleSpec, cancellationToken))
            .OrderBy(x => x.Name).ToList();
    }
}
