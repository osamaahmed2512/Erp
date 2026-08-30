using Application.Dtos.AccessControl;
using Application.Dtos.Response;
using Domain.Common;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.CreateCompanyRole;

public sealed class CreateCompanyRoleCommandHandler :
    IRequestHandler<CreateCompanyRoleCommand, BaseApiResponse<CompanyRoleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateCompanyRoleCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BaseApiResponse<CompanyRoleDto>> Handle(
        CreateCompanyRoleCommand request, CancellationToken cancellationToken)
    {
        var name = request.Dto.Name.Trim();
        if (name.Length is < 2 or > 80) return Fail(400, "Role name must be between 2 and 80 characters.");
        if (!await _unitOfWork.Repository<Domain.Entities.Company>()
                .AnyAsync(x => x.Id == request.CompanyId, cancellationToken))
            return Fail(404, "Company not found.");

        var normalized = name.ToUpperInvariant();
        if (await _unitOfWork.Repository<CompanyRole>().AnyAsync(x =>
                x.CompanyId == request.CompanyId && x.NormalizedName == normalized, cancellationToken))
            return Fail(409, "A role with this name already exists in the company.");

        var permissionIds = request.Dto.PermissionIds.Distinct().ToArray();
        var validation = await ValidateAuthorityAsync(request.ActorId, request.CompanyId, permissionIds, cancellationToken);
        if (validation is not null) return Fail(validation.StatusCode, validation.Message);

        var role = new CompanyRole
        {
            CompanyId = request.CompanyId,
            Name = name,
            NormalizedName = normalized,
            Permissions = permissionIds.Select(id => new CompanyRolePermission
            {
                PermissionDefinitionId = id
            }).ToList()
        };
        await _unitOfWork.Repository<CompanyRole>().AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return new BaseApiResponse<CompanyRoleDto>(201, "Role created successfully.",
            new CompanyRoleDto(role.Id, role.Name, role.IsSystem, permissionIds));
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
            SystemPermissionIds = x.SystemRoles.SelectMany(role => role.SystemRole.Permissions)
                .Select(item => item.PermissionDefinitionId).ToList(),
            DeniedIds = x.PermissionOverrides.Select(item => item.PermissionDefinitionId).ToList()
        }, actorSpec, cancellationToken);
        if (actor is null) return BaseApiResponse.Fail(403, "Access denied.");
        if (actor.IsRootSuperAdmin) return null;
        if (await _unitOfWork.Repository<Domain.Entities.Company>()
                .AnyAsync(x => x.Id == companyId && x.OwnerId == actorId, cancellationToken))
            return permissionIds.All(actor.CompanyPermissionIds.Contains)
                ? null
                : BaseApiResponse.Fail(403, "You cannot grant a permission outside the company's enabled modules.");
        if (actor.AccountType != AccountType.System) return BaseApiResponse.Fail(403, "Only the company owner can manage roles.");
        var effective = EffectivePermissionResolver.Resolve(
            actor.SystemPermissionIds.Select(x => x.ToString()), actor.DeniedIds.Select(x => x.ToString()));
        return permissionIds.All(x => effective.Contains(x.ToString()))
            ? null
            : BaseApiResponse.Fail(403, "You cannot grant a permission you do not have.");
    }

    private static BaseApiResponse<CompanyRoleDto> Fail(int code, string message) => new(code, message);
}
