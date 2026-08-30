using Application.Dtos.Response;
using Domain.Common;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using Domain.Specification.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.UpdateCompanyRole;

public sealed class UpdateCompanyRoleCommandHandler : IRequestHandler<UpdateCompanyRoleCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateCompanyRoleCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BaseApiResponse> Handle(UpdateCompanyRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Repository<CompanyRole>()
            .GetByIdSpecAsync(new CompanyRoleWithPermissionsSpecification(request.CompanyId, request.RoleId), cancellationToken);
        if (role is null) return BaseApiResponse.Fail(404, "Role not found.");
        if (role.IsSystem && !await _unitOfWork.Repository<ApplicationUser>()
                .AnyAsync(x => x.Id == request.ActorId && x.IsRootSuperAdmin, cancellationToken))
            return BaseApiResponse.Fail(409, "Protected company roles can only be changed by Root System Admin.");

        var name = request.Dto.Name.Trim();
        if (name.Length is < 2 or > 80) return BaseApiResponse.Fail(400, "Role name must be between 2 and 80 characters.");
        var normalized = name.ToUpperInvariant();
        if (role.IsSystem && role.NormalizedName == "OWNER" && normalized != "OWNER")
            return BaseApiResponse.Fail(409, "The protected Owner role cannot be renamed.");
        if (await _unitOfWork.Repository<CompanyRole>().AnyAsync(x =>
                x.CompanyId == request.CompanyId && x.Id != request.RoleId &&
                x.NormalizedName == normalized, cancellationToken))
            return BaseApiResponse.Fail(409, "A role with this name already exists in the company.");

        var permissionIds = request.Dto.PermissionIds.Distinct().ToArray();
        if (role.IsSystem && role.NormalizedName == "OWNER")
        {
            var accessControlSpec = new BaseSpecifications<PermissionDefinition>(x =>
                x.IsActive && x.SystemPage.IsActive && x.SystemPage.Key == "AccessControl");
            var requiredIds = await _unitOfWork.Repository<PermissionDefinition>()
                .GetProjectedAsync(x => x.Id, accessControlSpec, cancellationToken);
            permissionIds = permissionIds.Concat(requiredIds).Distinct().ToArray();
        }
        var validation = await ValidateAuthorityAsync(request.ActorId, request.CompanyId, permissionIds, cancellationToken);
        if (validation is not null) return validation;

        var existingPermissionIds = role.Permissions
            .Select(x => x.PermissionDefinitionId)
            .ToHashSet();
        var requestedPermissionIds = permissionIds.ToHashSet();

        foreach (var item in role.Permissions.Where(x =>
                     !requestedPermissionIds.Contains(x.PermissionDefinitionId)).ToList())
            await _unitOfWork.Repository<CompanyRolePermission>().DeleteAsync(item);

        var permissionsToAdd = requestedPermissionIds
            .Except(existingPermissionIds)
            .Select(permissionId => new CompanyRolePermission
            {
                CompanyRoleId = role.Id,
                PermissionDefinitionId = permissionId
            });
        await _unitOfWork.Repository<CompanyRolePermission>()
            .AddRangeAsync(permissionsToAdd, cancellationToken);

        role.Name = name;
        role.NormalizedName = normalized;
        role.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(200, "Role updated successfully.");
    }

    private async Task<BaseApiResponse?> ValidateAuthorityAsync(
        Guid actorId, Guid companyId, IReadOnlyCollection<Guid> permissionIds, CancellationToken cancellationToken)
    {
        var permissionSpec = new BaseSpecifications<PermissionDefinition>(x => x.IsActive &&
            permissionIds.Contains(x.Id) &&
            (x.SystemPage.Audience == PageAudience.Company || x.SystemPage.Audience == PageAudience.Both));
        var validIds = await _unitOfWork.Repository<PermissionDefinition>()
            .GetProjectedAsync(x => x.Id, permissionSpec, cancellationToken);
        if (validIds.Count != permissionIds.Count)
            return BaseApiResponse.Fail(400, "One or more permissions are invalid for company roles.");

        var actorSpec = new BaseSpecifications<ApplicationUser>(x => x.Id == actorId);
        var actor = await _unitOfWork.Repository<ApplicationUser>().GetSingleProjectedAsync(x => new
        {
            x.AccountType,
            x.IsRootSuperAdmin,
            CompanyPermissionIds = x.CompanyRoles
                .Where(role => role.CompanyRole.CompanyId == companyId)
                .SelectMany(role => role.CompanyRole.Permissions)
                .Select(item => item.PermissionDefinitionId).ToList(),
            Grants = x.SystemRoles.SelectMany(role => role.SystemRole.Permissions)
                .Select(item => item.PermissionDefinitionId).ToList(),
            Denies = x.PermissionOverrides.Select(item => item.PermissionDefinitionId).ToList()
        }, actorSpec, cancellationToken);
        if (actor is null) return BaseApiResponse.Fail(403, "Access denied.");
        if (actor.IsRootSuperAdmin) return null;
        if (await _unitOfWork.Repository<Domain.Entities.Company>()
                .AnyAsync(x => x.Id == companyId && x.OwnerId == actorId, cancellationToken))
            return permissionIds.All(actor.CompanyPermissionIds.Contains)
                ? null
                : BaseApiResponse.Fail(403, "You cannot grant a permission outside the company's enabled modules.");
        if (actor.AccountType != AccountType.System) return BaseApiResponse.Fail(403, "Only the company owner can manage roles.");
        var effective = actor.Grants.Except(actor.Denies).ToHashSet();
        return permissionIds.All(effective.Contains)
            ? null
            : BaseApiResponse.Fail(403, "You cannot grant a permission you do not have.");
    }
}
