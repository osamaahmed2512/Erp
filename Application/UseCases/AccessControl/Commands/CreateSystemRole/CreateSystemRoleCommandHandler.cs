using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.CreateSystemRole;

public sealed class CreateSystemRoleCommandHandler :
    IRequestHandler<CreateSystemRoleCommand, BaseApiResponse<SystemRoleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateSystemRoleCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BaseApiResponse<SystemRoleDto>> Handle(CreateSystemRoleCommand request, CancellationToken cancellationToken)
    {
        if (!await IsRootAsync(request.ActorId, cancellationToken)) return new(403, "Only Root System Admin can create system roles.");
        var name = request.Dto.Name.Trim();
        if (name.Length is < 2 or > 80) return new(400, "Role name must be between 2 and 80 characters.");
        var normalized = name.ToUpperInvariant();
        if (await _unitOfWork.Repository<SystemRole>().AnyAsync(x => x.NormalizedName == normalized, cancellationToken))
            return new(409, "A system role with this name already exists.");

        var permissionIds = request.Dto.PermissionIds.Distinct().ToArray();
        var spec = new BaseSpecifications<PermissionDefinition>(x => x.IsActive && permissionIds.Contains(x.Id) &&
            (x.SystemPage.Audience == PageAudience.System || x.SystemPage.Audience == PageAudience.Both));
        if (await _unitOfWork.Repository<PermissionDefinition>().CountWithSpec(spec, cancellationToken) != permissionIds.Length)
            return new(400, "One or more permissions are invalid for system roles.");

        var role = new SystemRole
        {
            Name = name,
            NormalizedName = normalized,
            Permissions = permissionIds.Select(id => new SystemRolePermission { PermissionDefinitionId = id }).ToList()
        };
        await _unitOfWork.Repository<SystemRole>().AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return new BaseApiResponse<SystemRoleDto>(201, "System role created successfully.",
            new SystemRoleDto(role.Id, role.Name, role.IsProtected, permissionIds));
    }

    private Task<bool> IsRootAsync(Guid actorId, CancellationToken ct) =>
        _unitOfWork.Repository<ApplicationUser>().AnyAsync(x => x.Id == actorId && x.IsRootSuperAdmin, ct);
}
