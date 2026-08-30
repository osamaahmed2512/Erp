using Application.Dtos.Response;
using Application.Interfaces.ExternalServices;
using Domain.Common;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification;
using Domain.Specification.Company;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.CreateCompanyWithOwner;

public sealed class CreateCompanyWithOwnerCommandHandler :
    IRequestHandler<CreateCompanyWithOwnerCommand, BaseApiResponse<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    public CreateCompanyWithOwnerCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<BaseApiResponse<Guid>> Handle(
        CreateCompanyWithOwnerCommand request, CancellationToken cancellationToken)
    {
        if (!await CanCreateCompanyAsync(request.ActorId, cancellationToken))
            return new(403, "You do not have permission to create companies.");
        var dto = request.Dto;
        var duplicate = await _unitOfWork.Repository<Domain.Entities.Company>()
            .AnyAsync(new CompanySpecification(dto.Phone.Trim(), dto.Email.Trim()), cancellationToken);
        if (duplicate) return new(409, "Company email or phone already exists.");
        if (await _identityService.FindByEmailAsync(dto.OwnerEmail.Trim()) is not null)
            return new(409, "Owner email already exists.");
        if (!string.IsNullOrWhiteSpace(dto.OwnerPhone) &&
            await _identityService.FindByPhoneAsync(dto.OwnerPhone.Trim()) is not null)
            return new(409, "Owner phone number already exists.");

        var selectedModules = (dto.EnabledModules ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (selectedModules.Length == 0)
            return new(400, "Select at least one company module.");
        var companyPageSpec = new BaseSpecifications<SystemPage>(x => x.IsActive &&
            x.Key != "AccessControl" &&
            (x.Audience == PageAudience.Company || x.Audience == PageAudience.Both));
        var availableModules = (await _unitOfWork.Repository<SystemPage>()
                .GetProjectedAsync(x => x.Module, companyPageSpec, cancellationToken))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (selectedModules.Any(selected =>
                !availableModules.Contains(selected, StringComparer.OrdinalIgnoreCase)))
            return new(400, "One or more selected company modules are invalid.");

        var normalizedModules = availableModules
            .Where(available => selectedModules.Contains(available, StringComparer.OrdinalIgnoreCase))
            .ToArray();
        var ownerPermissionSpec = new BaseSpecifications<PermissionDefinition>(x => x.IsActive &&
            x.SystemPage.IsActive &&
            (x.SystemPage.Audience == PageAudience.Company || x.SystemPage.Audience == PageAudience.Both) &&
            (x.SystemPage.Key == "AccessControl" || normalizedModules.Contains(x.SystemPage.Module)));
        var ownerPermissionIds = await _unitOfWork.Repository<PermissionDefinition>()
            .GetProjectedAsync(x => x.Id, ownerPermissionSpec, cancellationToken);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var ownerId = await _identityService.CreateUserAsync(
                dto.OwnerEmail.Trim(), dto.OwnerPassword, SystemRoles.Owner.ToString(),
                dto.OwnerFirstName.Trim(), dto.OwnerLastName.Trim(), dto.OwnerPhone?.Trim(),
                AccountType.System);
            var company = new Domain.Entities.Company
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim(),
                Phone = dto.Phone.Trim(),
                Description = dto.Description,
                Country = dto.Country,
                City = dto.City,
                Address = dto.Address,
                PostalCode = dto.PostalCode,
                TaxNumber = dto.TaxNumber,
                CommercialRegistration = dto.CommercialRegistration,
                Website = dto.Website,
                OwnerId = ownerId
            };
            await _unitOfWork.Repository<Domain.Entities.Company>().AddAsync(company, cancellationToken);
            var ownerRole = new CompanyRole
            {
                CompanyId = company.Id,
                Name = "Owner",
                NormalizedName = "OWNER",
                IsSystem = true,
                Permissions = ownerPermissionIds.Select(permissionId =>
                    new CompanyRolePermission { PermissionDefinitionId = permissionId }).ToList()
            };
            await _unitOfWork.Repository<CompanyRole>().AddAsync(ownerRole, cancellationToken);
            await _unitOfWork.Repository<CompanyUserRole>().AddAsync(
                new CompanyUserRole
                {
                    UserId = ownerId,
                    CompanyRoleId = ownerRole.Id
                }, cancellationToken);
            await _unitOfWork.SaveChangeAsync(cancellationToken);
            await _identityService.AssignCompanyAsync(ownerId, company.Id);
            if (!await _unitOfWork.CommitAsync()) throw new InvalidOperationException("Could not commit company creation.");
            return new BaseApiResponse<Guid>(201, "Company and owner created successfully.", company.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new BaseApiResponse<Guid>(400, ex.Message);
        }
    }

    private async Task<bool> CanCreateCompanyAsync(Guid actorId, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Repository<ApplicationUser>()
                .AnyAsync(x => x.Id == actorId && x.IsRootSuperAdmin, cancellationToken)) return true;
        return await _unitOfWork.Repository<SystemUserRole>().AnyAsync(x =>
            x.UserId == actorId && x.SystemRole.Permissions.Any(p =>
                p.PermissionDefinition.Key == "Companies.Create"), cancellationToken) &&
            !await _unitOfWork.Repository<UserPermissionOverride>().AnyAsync(x =>
                x.UserId == actorId && x.PermissionDefinition.Key == "Companies.Create", cancellationToken);
    }
}
