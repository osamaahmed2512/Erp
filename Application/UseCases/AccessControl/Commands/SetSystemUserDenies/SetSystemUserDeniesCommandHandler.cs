using Application.Dtos.Response;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.SetSystemUserDenies;

public sealed class SetSystemUserDeniesCommandHandler : IRequestHandler<SetSystemUserDeniesCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetSystemUserDeniesCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BaseApiResponse> Handle(SetSystemUserDeniesCommand request, CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.Repository<ApplicationUser>()
                .AnyAsync(x => x.Id == request.ActorId && x.IsRootSuperAdmin, cancellationToken))
            return BaseApiResponse.Fail(403, "Only Root System Admin can set system-user denies.");
        if (!await _unitOfWork.Repository<ApplicationUser>().AnyAsync(x =>
                x.Id == request.UserId && x.AccountType == AccountType.System && !x.IsRootSuperAdmin, cancellationToken))
            return BaseApiResponse.Fail(404, "System user not found or protected.");

        var ids = request.Dto.DeniedPermissionIds.Distinct().ToArray();
        var permissionSpec = new BaseSpecifications<PermissionDefinition>(x => x.IsActive && ids.Contains(x.Id) &&
            (x.SystemPage.Audience == PageAudience.System || x.SystemPage.Audience == PageAudience.Both));
        if (await _unitOfWork.Repository<PermissionDefinition>().CountWithSpec(permissionSpec, cancellationToken) != ids.Length)
            return BaseApiResponse.Fail(400, "One or more denied permissions are invalid for system users.");

        var overrideSpec = new BaseSpecifications<UserPermissionOverride>(x => x.UserId == request.UserId);
        var existing = await _unitOfWork.Repository<UserPermissionOverride>()
            .GetAllWithSpecAsync(overrideSpec, false, cancellationToken);
        foreach (var item in existing) await _unitOfWork.Repository<UserPermissionOverride>().DeleteAsync(item);
        await _unitOfWork.Repository<UserPermissionOverride>().AddRangeAsync(ids.Select(permissionId =>
            new UserPermissionOverride
            {
                UserId = request.UserId,
                PermissionDefinitionId = permissionId,
                Effect = PermissionEffect.Deny
            }), cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(200, "System user denies updated successfully.");
    }
}
