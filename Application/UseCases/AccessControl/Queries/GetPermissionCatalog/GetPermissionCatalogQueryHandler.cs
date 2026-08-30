using Application.Dtos.AccessControl;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetPermissionCatalog;

public sealed class GetPermissionCatalogQueryHandler : IRequestHandler<GetPermissionCatalogQuery, IReadOnlyList<SystemPageDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetPermissionCatalogQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<SystemPageDto>> Handle(GetPermissionCatalogQuery request, CancellationToken cancellationToken)
    {
        var userSpec = new BaseSpecifications<ApplicationUser>(x => x.Id == request.ActorId);
        var actor = await _unitOfWork.Repository<ApplicationUser>().GetSingleProjectedAsync(x => new
        {
            x.AccountType,
            x.CompanyId,
            x.IsRootSuperAdmin,
            CompanyPermissionIds = x.CompanyRoles
                .Where(role => role.CompanyRole.CompanyId == request.CompanyId)
                .SelectMany(role => role.CompanyRole.Permissions)
                .Select(item => item.PermissionDefinitionId)
                .ToList()
        }, userSpec, cancellationToken);
        if (actor is null) return [];

        var isOwner = actor.AccountType == AccountType.Company && actor.CompanyId == request.CompanyId &&
            await _unitOfWork.Repository<Domain.Entities.Company>()
                .AnyAsync(x => x.Id == request.CompanyId && x.OwnerId == request.ActorId, cancellationToken);
        if (!actor.IsRootSuperAdmin && !isOwner) return [];

        // The requested dashboard determines the catalog audience. Root and support
        // system users can open a company dashboard, but system-only permissions
        // must never be assignable to a company role.
        var audience = request.CompanyId == Guid.Empty
            ? PageAudience.System
            : PageAudience.Company;
        var pageSpec = new BaseSpecifications<SystemPage>(x => x.IsActive &&
            (x.Audience == PageAudience.Both || x.Audience == audience));
        var pages = (await _unitOfWork.Repository<SystemPage>().GetProjectedAsync(x =>
            new SystemPageDto(x.Id, x.Key, x.Name, x.Module, x.Category, x.Route, x.Icon, x.DisplayOrder,
                x.Audience, x.Permissions.Where(p => p.IsActive)
                    .Select(p => new PermissionDto(p.Id, p.Key, p.Action)).ToList()),
            pageSpec, cancellationToken)).OrderBy(x => x.DisplayOrder).ToList();
        if (actor.IsRootSuperAdmin || !isOwner) return pages;

        var allowedIds = actor.CompanyPermissionIds.ToHashSet();
        return pages.Select(page => page with
            {
                Permissions = page.Permissions.Where(permission => allowedIds.Contains(permission.Id)).ToList()
            })
            .Where(page => page.Permissions.Any(permission => permission.Action == "View"))
            .ToList();
    }
}
