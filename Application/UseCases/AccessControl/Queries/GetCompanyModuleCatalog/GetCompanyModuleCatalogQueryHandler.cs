using Application.Dtos.AccessControl;
using Domain.Common;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetCompanyModuleCatalog;

public sealed class GetCompanyModuleCatalogQueryHandler :
    IRequestHandler<GetCompanyModuleCatalogQuery, IReadOnlyList<CompanyModuleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCompanyModuleCatalogQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<CompanyModuleDto>> Handle(
        GetCompanyModuleCatalogQuery request, CancellationToken cancellationToken)
    {
        if (!await CanCreateCompanyAsync(request.ActorId, cancellationToken)) return [];

        var pageSpec = new BaseSpecifications<SystemPage>(x => x.IsActive &&
            x.Key != "AccessControl" &&
            (x.Audience == PageAudience.Company || x.Audience == PageAudience.Both));
        var pages = await _unitOfWork.Repository<SystemPage>().GetProjectedAsync(x =>
            new SystemPageDto(x.Id, x.Key, x.Name, x.Module, x.Category, x.Route, x.Icon, x.DisplayOrder,
                x.Audience, x.Permissions.Where(p => p.IsActive)
                    .Select(p => new PermissionDto(p.Id, p.Key, p.Action)).ToList()),
            pageSpec, cancellationToken);

        return pages.GroupBy(x => x.Module)
            .Select(group => new CompanyModuleDto(group.Key,
                group.OrderBy(x => x.DisplayOrder).ToList()))
            .OrderBy(x => x.Name)
            .ToList();
    }

    private async Task<bool> CanCreateCompanyAsync(Guid actorId, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Repository<ApplicationUser>()
                .AnyAsync(x => x.Id == actorId && x.IsRootSuperAdmin, cancellationToken)) return true;
        return await _unitOfWork.Repository<SystemUserRole>().AnyAsync(x =>
            x.UserId == actorId && x.SystemRole.Permissions.Any(p =>
                p.PermissionDefinition.Key == Permissions.Companies_Create), cancellationToken) &&
            !await _unitOfWork.Repository<UserPermissionOverride>().AnyAsync(x =>
                x.UserId == actorId && x.PermissionDefinition.Key == Permissions.Companies_Create,
                cancellationToken);
    }
}
