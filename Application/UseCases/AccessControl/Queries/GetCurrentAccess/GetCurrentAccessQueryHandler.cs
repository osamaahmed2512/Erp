using Application.Dtos.AccessControl;
using Domain.Common;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetCurrentAccess;

public sealed class GetCurrentAccessQueryHandler : IRequestHandler<GetCurrentAccessQuery, CurrentAccessDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetCurrentAccessQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<CurrentAccessDto> Handle(GetCurrentAccessQuery request, CancellationToken cancellationToken)
    {
        var userSpec = new BaseSpecifications<ApplicationUser>(x => x.Id == request.UserId);
        var user = await _unitOfWork.Repository<ApplicationUser>().GetSingleProjectedAsync(x => new
        {
            x.AccountType,
            x.CompanyId,
            x.IsRootSuperAdmin,
            CompanyRoleKeys = x.CompanyRoles.SelectMany(role => role.CompanyRole.Permissions)
                .Where(item => item.PermissionDefinition.IsActive)
                .Select(item => item.PermissionDefinition.Key).ToList(),
            SystemRoleKeys = x.SystemRoles.SelectMany(role => role.SystemRole.Permissions)
                .Where(item => item.PermissionDefinition.IsActive)
                .Select(item => item.PermissionDefinition.Key).ToList(),
            DeniedKeys = x.PermissionOverrides
                .Where(item => item.Effect == PermissionEffect.Deny && item.PermissionDefinition.IsActive)
                .Select(item => item.PermissionDefinition.Key).ToList()
        }, userSpec, cancellationToken) ?? throw new UnauthorizedAccessException("User account not found.");

        var companies = user.AccountType == AccountType.System
            ? (IReadOnlyList<AccessCompanyDto>)(await _unitOfWork.Repository<Domain.Entities.Company>()
                .GetProjectedAsync(x => new AccessCompanyDto(x.Id, x.Name), cancellationToken: cancellationToken))
                .OrderBy(x => x.Name).ToList()
            : await GetCompanyAsync(user.CompanyId, cancellationToken);

        var companyId = user.AccountType == AccountType.Company
            ? user.CompanyId
            : request.CompanyId.HasValue && companies.Any(x => x.Id == request.CompanyId)
                ? request.CompanyId
                : companies.Select(x => (Guid?)x.Id).FirstOrDefault();
        var isOwner = user.AccountType == AccountType.Company && companyId.HasValue &&
            await _unitOfWork.Repository<Domain.Entities.Company>()
                .AnyAsync(x => x.Id == companyId && x.OwnerId == request.UserId, cancellationToken);

        var audience = user.AccountType == AccountType.System ? PageAudience.System : PageAudience.Company;
        IReadOnlySet<string> effective;
        if (user.IsRootSuperAdmin)
        {
            var permissionSpec = new BaseSpecifications<PermissionDefinition>(x => x.IsActive &&
                (x.SystemPage.Audience == PageAudience.Both || x.SystemPage.Audience == audience));
            effective = (await _unitOfWork.Repository<PermissionDefinition>()
                .GetProjectedAsync(x => x.Key, permissionSpec, cancellationToken)).ToHashSet(StringComparer.Ordinal);
        }
        else
        {
            var grants = user.AccountType == AccountType.System ? user.SystemRoleKeys : user.CompanyRoleKeys;
            effective = EffectivePermissionResolver.Resolve(grants, user.DeniedKeys);
        }

        var pageSpec = new BaseSpecifications<SystemPage>(x => x.IsActive &&
            (x.Audience == PageAudience.Both || x.Audience == audience));
        var pages = (await _unitOfWork.Repository<SystemPage>().GetProjectedAsync(x =>
            new SystemPageDto(x.Id, x.Key, x.Name, x.Module, x.Category, x.Route, x.Icon, x.DisplayOrder,
                x.Audience, x.Permissions.Where(p => p.IsActive)
                    .Select(p => new PermissionDto(p.Id, p.Key, p.Action)).ToList()),
            pageSpec, cancellationToken))
            .Where(x => effective.Contains($"{x.Key}.View"))
            .Select(x => x with
            {
                Permissions = x.Permissions.Where(p => effective.Contains(p.Key)).ToList()
            })
            .OrderBy(x => x.DisplayOrder).ToList();

        return new CurrentAccessDto(user.AccountType, user.IsRootSuperAdmin, isOwner, companyId,
            companies, effective.OrderBy(x => x).ToList(), pages);
    }

    private async Task<IReadOnlyList<AccessCompanyDto>> GetCompanyAsync(
        Guid? companyId, CancellationToken cancellationToken)
    {
        if (!companyId.HasValue) return [];
        var spec = new BaseSpecifications<Domain.Entities.Company>(x => x.Id == companyId);
        return await _unitOfWork.Repository<Domain.Entities.Company>()
            .GetProjectedAsync(x => new AccessCompanyDto(x.Id, x.Name), spec, cancellationToken);
    }
}
