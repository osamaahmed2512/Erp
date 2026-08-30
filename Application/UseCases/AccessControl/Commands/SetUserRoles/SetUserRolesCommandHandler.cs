using Application.Dtos.Response;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.SetUserRoles;

public sealed class SetUserRolesCommandHandler : IRequestHandler<SetUserRolesCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetUserRolesCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BaseApiResponse> Handle(SetUserRolesCommand request, CancellationToken cancellationToken)
    {
        var targetValid = await _unitOfWork.Repository<ApplicationUser>().AnyAsync(x =>
            x.Id == request.UserId && x.AccountType == AccountType.Company &&
            x.CompanyId == request.CompanyId && !x.IsRootSuperAdmin, cancellationToken);
        if (!targetValid) return BaseApiResponse.Fail(404, "Company user not found.");
        if (await _unitOfWork.Repository<Domain.Entities.Company>()
                .AnyAsync(x => x.Id == request.CompanyId && x.OwnerId == request.UserId, cancellationToken))
            return BaseApiResponse.Fail(409, "The company owner's protected role assignment cannot be modified.");
        if (!await CanManageAsync(request.ActorId, request.CompanyId, cancellationToken))
            return BaseApiResponse.Fail(403, "Only the company owner or an authorized system user can assign roles.");

        var roleIds = request.Dto.RoleIds.Distinct().ToArray();
        var roleSpec = new BaseSpecifications<CompanyRole>(x =>
            x.CompanyId == request.CompanyId && roleIds.Contains(x.Id));
        var validRoleIds = await _unitOfWork.Repository<CompanyRole>()
            .GetProjectedAsync(x => x.Id, roleSpec, cancellationToken);
        if (validRoleIds.Count != roleIds.Length)
            return BaseApiResponse.Fail(400, "One or more roles belong to another company.");

        var assignmentSpec = new BaseSpecifications<CompanyUserRole>(x => x.UserId == request.UserId);
        var existing = await _unitOfWork.Repository<CompanyUserRole>()
            .GetAllWithSpecAsync(assignmentSpec, false, cancellationToken);
        foreach (var item in existing) await _unitOfWork.Repository<CompanyUserRole>().DeleteAsync(item);
        await _unitOfWork.Repository<CompanyUserRole>().AddRangeAsync(roleIds.Select(roleId =>
            new CompanyUserRole { UserId = request.UserId, CompanyRoleId = roleId }), cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(200, "User roles updated successfully.");
    }

    private async Task<bool> CanManageAsync(Guid actorId, Guid companyId, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Repository<ApplicationUser>()
                .AnyAsync(x => x.Id == actorId && x.IsRootSuperAdmin, cancellationToken)) return true;
        if (await _unitOfWork.Repository<Domain.Entities.Company>()
                .AnyAsync(x => x.Id == companyId && x.OwnerId == actorId, cancellationToken)) return true;
        var permissionSpec = new BaseSpecifications<ApplicationUser>(x => x.Id == actorId && x.AccountType == AccountType.System);
        return await _unitOfWork.Repository<ApplicationUser>().GetSingleProjectedAsync(x =>
            x.SystemRoles.SelectMany(role => role.SystemRole.Permissions)
                .Any(item => item.PermissionDefinition.Key == "AccessControl.Edit") &&
            !x.PermissionOverrides.Any(item => item.PermissionDefinition.Key == "AccessControl.Edit"),
            permissionSpec, cancellationToken);
    }
}
