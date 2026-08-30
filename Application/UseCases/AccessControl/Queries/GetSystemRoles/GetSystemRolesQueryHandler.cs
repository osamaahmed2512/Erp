using Application.Dtos.AccessControl;
using Domain.Entities;
using Domain.Interfaces.UnitOfWork;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetSystemRoles;

public sealed class GetSystemRolesQueryHandler : IRequestHandler<GetSystemRolesQuery, IReadOnlyList<SystemRoleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetSystemRolesQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<SystemRoleDto>> Handle(GetSystemRolesQuery request, CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.Repository<ApplicationUser>()
                .AnyAsync(x => x.Id == request.ActorId && x.IsRootSuperAdmin, cancellationToken)) return [];
        return (await _unitOfWork.Repository<SystemRole>().GetProjectedAsync(x =>
            new SystemRoleDto(x.Id, x.Name, x.IsProtected,
                x.Permissions.Select(item => item.PermissionDefinitionId).ToList()),
            cancellationToken: cancellationToken)).OrderBy(x => x.Name).ToList();
    }
}
