using Application.Dtos.Response;
using Domain.Entities;
using Domain.Interfaces.UnitOfWork;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.DeleteSystemRole;

public sealed class DeleteSystemRoleCommandHandler : IRequestHandler<DeleteSystemRoleCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteSystemRoleCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BaseApiResponse> Handle(DeleteSystemRoleCommand request, CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.Repository<ApplicationUser>()
                .AnyAsync(x => x.Id == request.ActorId && x.IsRootSuperAdmin, cancellationToken))
            return BaseApiResponse.Fail(403, "Only Root System Admin can delete system roles.");
        var role = await _unitOfWork.Repository<SystemRole>().GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null) return BaseApiResponse.Fail(404, "System role not found.");
        if (role.IsProtected) return BaseApiResponse.Fail(409, "Protected system roles cannot be deleted.");
        if (await _unitOfWork.Repository<SystemUserRole>()
                .AnyAsync(x => x.SystemRoleId == role.Id, cancellationToken))
            return BaseApiResponse.Fail(409, "The system role is assigned to one or more users.");
        await _unitOfWork.Repository<SystemRole>().DeleteAsync(role);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(200, "System role deleted successfully.");
    }
}
