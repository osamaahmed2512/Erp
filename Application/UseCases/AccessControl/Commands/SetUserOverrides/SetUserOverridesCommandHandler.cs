using Application.Dtos.Response;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.SetUserOverrides;

public sealed class SetUserOverridesCommandHandler : IRequestHandler<SetUserOverridesCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetUserOverridesCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BaseApiResponse> Handle(SetUserOverridesCommand request, CancellationToken cancellationToken)
    {
        var targetValid = await _unitOfWork.Repository<ApplicationUser>().AnyAsync(x =>
            x.Id == request.UserId && x.AccountType == AccountType.Company &&
            x.CompanyId == request.CompanyId && !x.IsRootSuperAdmin, cancellationToken);
        if (!targetValid) return BaseApiResponse.Fail(404, "Company user not found.");
        if (await _unitOfWork.Repository<Domain.Entities.Company>()
                .AnyAsync(x => x.Id == request.CompanyId && x.OwnerId == request.UserId, cancellationToken))
            return BaseApiResponse.Fail(409, "The company owner is protected and cannot receive direct denies.");
        if (!await CanManageAsync(request.ActorId, request.CompanyId, cancellationToken))
            return BaseApiResponse.Fail(403, "Only the company owner or an authorized system user can set denies.");

        var deniedIds = request.Dto.DeniedPermissionIds.Distinct().ToArray();
        var permissionSpec = new BaseSpecifications<PermissionDefinition>(x => x.IsActive &&
            deniedIds.Contains(x.Id) &&
            (x.SystemPage.Audience == PageAudience.Company || x.SystemPage.Audience == PageAudience.Both));
        var validIds = await _unitOfWork.Repository<PermissionDefinition>()
            .GetProjectedAsync(x => x.Id, permissionSpec, cancellationToken);
        if (validIds.Count != deniedIds.Length)
            return BaseApiResponse.Fail(400, "One or more denied permissions are invalid for company users.");

        var overrideSpec = new BaseSpecifications<UserPermissionOverride>(x => x.UserId == request.UserId);
        var existing = await _unitOfWork.Repository<UserPermissionOverride>()
            .GetAllWithSpecAsync(overrideSpec, false, cancellationToken);
        foreach (var item in existing) await _unitOfWork.Repository<UserPermissionOverride>().DeleteAsync(item);
        await _unitOfWork.Repository<UserPermissionOverride>().AddRangeAsync(deniedIds.Select(permissionId =>
            new UserPermissionOverride
            {
                UserId = request.UserId,
                PermissionDefinitionId = permissionId,
                Effect = PermissionEffect.Deny
            }), cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(200, "User denies updated successfully.");
    }

    private async Task<bool> CanManageAsync(Guid actorId, Guid companyId, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Repository<ApplicationUser>()
                .AnyAsync(x => x.Id == actorId && x.IsRootSuperAdmin, cancellationToken)) return true;
        if (await _unitOfWork.Repository<Domain.Entities.Company>()
                .AnyAsync(x => x.Id == companyId && x.OwnerId == actorId, cancellationToken)) return true;
        var actorSpec = new BaseSpecifications<ApplicationUser>(x => x.Id == actorId && x.AccountType == AccountType.System);
        return await _unitOfWork.Repository<ApplicationUser>().GetSingleProjectedAsync(x =>
            x.SystemRoles.SelectMany(role => role.SystemRole.Permissions)
                .Any(item => item.PermissionDefinition.Key == "AccessControl.Edit") &&
            !x.PermissionOverrides.Any(item => item.PermissionDefinition.Key == "AccessControl.Edit"),
            actorSpec, cancellationToken);
    }
}
