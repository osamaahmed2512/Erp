using Application.Dtos.Response;
using Domain.Entities;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.DeleteCompanyRole;

public sealed class DeleteCompanyRoleCommandHandler : IRequestHandler<DeleteCompanyRoleCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteCompanyRoleCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BaseApiResponse> Handle(DeleteCompanyRoleCommand request, CancellationToken cancellationToken)
    {
        var allowed = await _unitOfWork.Repository<ApplicationUser>()
            .AnyAsync(x => x.Id == request.ActorId && x.IsRootSuperAdmin, cancellationToken) ||
            await _unitOfWork.Repository<Domain.Entities.Company>()
                .AnyAsync(x => x.Id == request.CompanyId && x.OwnerId == request.ActorId, cancellationToken);
        if (!allowed) return BaseApiResponse.Fail(403, "Only the company owner or Root System Admin can delete roles.");

        var spec = new BaseSpecifications<CompanyRole>(x =>
            x.Id == request.RoleId && x.CompanyId == request.CompanyId);
        var role = await _unitOfWork.Repository<CompanyRole>().GetByIdSpecAsync(spec, cancellationToken);
        if (role is null) return BaseApiResponse.Fail(404, "Role not found.");
        if (role.IsSystem) return BaseApiResponse.Fail(409, "Protected company roles cannot be deleted.");
        if (await _unitOfWork.Repository<CompanyUserRole>()
                .AnyAsync(x => x.CompanyRoleId == role.Id, cancellationToken))
            return BaseApiResponse.Fail(409, "The role is assigned to one or more users.");

        await _unitOfWork.Repository<CompanyRole>().DeleteAsync(role);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(200, "Role deleted successfully.");
    }
}
