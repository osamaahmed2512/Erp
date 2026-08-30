using Application.Dtos.Response;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.SetSystemUserRoles;

public sealed class SetSystemUserRolesCommandHandler : IRequestHandler<SetSystemUserRolesCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetSystemUserRolesCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BaseApiResponse> Handle(SetSystemUserRolesCommand request, CancellationToken cancellationToken)
    {
        if (!await IsRootAsync(request.ActorId, cancellationToken))
            return BaseApiResponse.Fail(403, "Only Root System Admin can assign system roles.");
        if (!await _unitOfWork.Repository<ApplicationUser>().AnyAsync(x =>
                x.Id == request.UserId && x.AccountType == AccountType.System && !x.IsRootSuperAdmin, cancellationToken))
            return BaseApiResponse.Fail(404, "System user not found or protected.");

        var ids = request.Dto.RoleIds.Distinct().ToArray();
        var roleSpec = new BaseSpecifications<SystemRole>(x => ids.Contains(x.Id));
        if (await _unitOfWork.Repository<SystemRole>().CountWithSpec(roleSpec, cancellationToken) != ids.Length)
            return BaseApiResponse.Fail(400, "One or more system roles are invalid.");

        var assignmentSpec = new BaseSpecifications<SystemUserRole>(x => x.UserId == request.UserId);
        var existing = await _unitOfWork.Repository<SystemUserRole>()
            .GetAllWithSpecAsync(assignmentSpec, false, cancellationToken);
        foreach (var item in existing) await _unitOfWork.Repository<SystemUserRole>().DeleteAsync(item);
        await _unitOfWork.Repository<SystemUserRole>().AddRangeAsync(ids.Select(roleId =>
            new SystemUserRole { UserId = request.UserId, SystemRoleId = roleId }), cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(200, "System user roles updated successfully.");
    }

    private Task<bool> IsRootAsync(Guid actorId, CancellationToken ct) =>
        _unitOfWork.Repository<ApplicationUser>().AnyAsync(x => x.Id == actorId && x.IsRootSuperAdmin, ct);
}
