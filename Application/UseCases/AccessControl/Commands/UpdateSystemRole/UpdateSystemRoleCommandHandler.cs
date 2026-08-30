using Application.Dtos.Response;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using Domain.Specification.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.UpdateSystemRole;

public sealed class UpdateSystemRoleCommandHandler : IRequestHandler<UpdateSystemRoleCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateSystemRoleCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BaseApiResponse> Handle(UpdateSystemRoleCommand request, CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.Repository<ApplicationUser>()
                .AnyAsync(x => x.Id == request.ActorId && x.IsRootSuperAdmin, cancellationToken))
            return BaseApiResponse.Fail(403, "Only Root System Admin can update system roles.");
        var role = await _unitOfWork.Repository<SystemRole>()
            .GetByIdSpecAsync(new SystemRoleWithPermissionsSpecification(request.RoleId), cancellationToken);
        if (role is null) return BaseApiResponse.Fail(404, "System role not found.");
        if (role.IsProtected) return BaseApiResponse.Fail(409, "Protected system roles cannot be modified.");

        var name = request.Dto.Name.Trim();
        if (name.Length is < 2 or > 80) return BaseApiResponse.Fail(400, "Role name must be between 2 and 80 characters.");
        var normalized = name.ToUpperInvariant();
        if (await _unitOfWork.Repository<SystemRole>().AnyAsync(x =>
                x.Id != role.Id && x.NormalizedName == normalized, cancellationToken))
            return BaseApiResponse.Fail(409, "A system role with this name already exists.");

        var ids = request.Dto.PermissionIds.Distinct().ToArray();
        var permissionSpec = new BaseSpecifications<PermissionDefinition>(x => x.IsActive && ids.Contains(x.Id) &&
            (x.SystemPage.Audience == PageAudience.System || x.SystemPage.Audience == PageAudience.Both));
        if (await _unitOfWork.Repository<PermissionDefinition>().CountWithSpec(permissionSpec, cancellationToken) != ids.Length)
            return BaseApiResponse.Fail(400, "One or more permissions are invalid for system roles.");

        var existingPermissionIds = role.Permissions
            .Select(x => x.PermissionDefinitionId)
            .ToHashSet();
        var requestedPermissionIds = ids.ToHashSet();

        foreach (var item in role.Permissions.Where(x =>
                     !requestedPermissionIds.Contains(x.PermissionDefinitionId)).ToList())
            await _unitOfWork.Repository<SystemRolePermission>().DeleteAsync(item);

        var permissionsToAdd = requestedPermissionIds
            .Except(existingPermissionIds)
            .Select(permissionId => new SystemRolePermission
            {
                SystemRoleId = role.Id,
                PermissionDefinitionId = permissionId
            });
        await _unitOfWork.Repository<SystemRolePermission>()
            .AddRangeAsync(permissionsToAdd, cancellationToken);

        role.Name = name;
        role.NormalizedName = normalized;
        role.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(200, "System role updated successfully.");
    }
}
