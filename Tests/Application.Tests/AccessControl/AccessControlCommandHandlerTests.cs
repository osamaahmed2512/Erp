using Application.Dtos.AccessControl;
using Application.Tests.Fakes;
using Application.UseCases.AccessControl.Commands.SetSystemUserRoles;
using Application.UseCases.AccessControl.Commands.SetUserRoles;
using Application.UseCases.AccessControl.Commands.UpdateSystemRole;
using Application.UseCases.AccessControl.Commands.UpdateCompanyRole;
using Domain.Entities;
using Domain.Enum;

namespace Application.Tests.AccessControl;

public sealed class AccessControlCommandHandlerTests
{
    [Fact]
    public async Task SetCompanyRoles_ReturnsNotFound_ForUserOutsideCompany()
    {
        var actorId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var unitOfWork = new FakeUnitOfWork().Seed(
            new ApplicationUser { Id = actorId, IsRootSuperAdmin = true, AccountType = AccountType.System },
            new ApplicationUser { Id = userId, AccountType = AccountType.Company, CompanyId = Guid.NewGuid() });
        var handler = new SetUserRolesCommandHandler(unitOfWork);

        var response = await handler.Handle(
            new SetUserRolesCommand(actorId, companyId, userId, new SetUserRolesDto([])),
            CancellationToken.None);

        Assert.Equal(404, response.StatusCode);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task SetCompanyRoles_RejectsCompanyOwner()
    {
        var actorId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var unitOfWork = new FakeUnitOfWork()
            .Seed(new ApplicationUser { Id = actorId, IsRootSuperAdmin = true, AccountType = AccountType.System },
                new ApplicationUser { Id = ownerId, AccountType = AccountType.Company, CompanyId = companyId })
            .Seed(new Domain.Entities.Company
            {
                Id = companyId, OwnerId = ownerId, Name = "ERP", Email = "erp@test.com", Phone = "123"
            });
        var handler = new SetUserRolesCommandHandler(unitOfWork);

        var response = await handler.Handle(
            new SetUserRolesCommand(actorId, companyId, ownerId, new SetUserRolesDto([])),
            CancellationToken.None);

        Assert.Equal(409, response.StatusCode);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task SetCompanyRoles_RejectsRoleFromAnotherCompany()
    {
        var actorId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var unitOfWork = new FakeUnitOfWork()
            .Seed(new ApplicationUser { Id = actorId, IsRootSuperAdmin = true, AccountType = AccountType.System },
                new ApplicationUser { Id = userId, AccountType = AccountType.Company, CompanyId = companyId })
            .Seed(new Domain.Entities.Company
            {
                Id = companyId, OwnerId = Guid.NewGuid(), Name = "ERP", Email = "erp@test.com", Phone = "123"
            })
            .Seed(new CompanyRole { Id = roleId, CompanyId = Guid.NewGuid(), Name = "HR", NormalizedName = "HR" });
        var handler = new SetUserRolesCommandHandler(unitOfWork);

        var response = await handler.Handle(
            new SetUserRolesCommand(actorId, companyId, userId, new SetUserRolesDto([roleId])),
            CancellationToken.None);

        Assert.Equal(400, response.StatusCode);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task SetSystemRoles_RejectsRootTarget()
    {
        var actorId = Guid.NewGuid();
        var rootId = Guid.NewGuid();
        var unitOfWork = new FakeUnitOfWork().Seed(
            new ApplicationUser { Id = actorId, IsRootSuperAdmin = true, AccountType = AccountType.System },
            new ApplicationUser { Id = rootId, IsRootSuperAdmin = true, AccountType = AccountType.System });
        var handler = new SetSystemUserRolesCommandHandler(unitOfWork);

        var response = await handler.Handle(
            new SetSystemUserRolesCommand(actorId, rootId, new SetUserRolesDto([])),
            CancellationToken.None);

        Assert.Equal(404, response.StatusCode);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task UpdateSystemRole_KeepsExistingPermission_AndAddsOnlyTheDifference()
    {
        var actorId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var existingPermissionId = Guid.NewGuid();
        var addedPermissionId = Guid.NewGuid();
        var existingLink = new SystemRolePermission
        {
            SystemRoleId = roleId,
            PermissionDefinitionId = existingPermissionId
        };
        var role = new SystemRole
        {
            Id = roleId,
            Name = "Support Admin",
            NormalizedName = "SUPPORT ADMIN",
            Permissions = [existingLink]
        };
        var bothPage = new SystemPage { Audience = PageAudience.Both, IsActive = true };
        var unitOfWork = new FakeUnitOfWork()
            .Seed(new ApplicationUser
            {
                Id = actorId,
                IsRootSuperAdmin = true,
                AccountType = AccountType.System
            })
            .Seed(role)
            .Seed(
                new PermissionDefinition
                {
                    Id = existingPermissionId,
                    IsActive = true,
                    SystemPage = bothPage
                },
                new PermissionDefinition
                {
                    Id = addedPermissionId,
                    IsActive = true,
                    SystemPage = bothPage
                })
            .Seed(existingLink);
        var handler = new UpdateSystemRoleCommandHandler(unitOfWork);

        var response = await handler.Handle(
            new UpdateSystemRoleCommand(actorId, roleId,
                new UpdateSystemRoleDto("Support Admin", [existingPermissionId, addedPermissionId])),
            CancellationToken.None);

        var savedLinks = await unitOfWork.Repository<SystemRolePermission>()
            .GetAllAsync(CancellationToken.None);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(2, savedLinks.Count);
        Assert.Contains(existingLink, savedLinks);
        Assert.Equal(1, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task UpdateCompanyRole_KeepsExistingPermission_AndAddsOnlyTheDifference()
    {
        var actorId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var existingPermissionId = Guid.NewGuid();
        var addedPermissionId = Guid.NewGuid();
        var existingLink = new CompanyRolePermission
        {
            CompanyRoleId = roleId,
            PermissionDefinitionId = existingPermissionId
        };
        var role = new CompanyRole
        {
            Id = roleId,
            CompanyId = companyId,
            Name = "Manager",
            NormalizedName = "MANAGER",
            Permissions = [existingLink]
        };
        var companyPage = new SystemPage { Audience = PageAudience.Company, IsActive = true };
        var unitOfWork = new FakeUnitOfWork()
            .Seed(new ApplicationUser
            {
                Id = actorId,
                IsRootSuperAdmin = true,
                AccountType = AccountType.System
            })
            .Seed(role)
            .Seed(
                new PermissionDefinition
                {
                    Id = existingPermissionId,
                    IsActive = true,
                    SystemPage = companyPage
                },
                new PermissionDefinition
                {
                    Id = addedPermissionId,
                    IsActive = true,
                    SystemPage = companyPage
                })
            .Seed(existingLink);
        var handler = new UpdateCompanyRoleCommandHandler(unitOfWork);

        var response = await handler.Handle(
            new UpdateCompanyRoleCommand(actorId, companyId, roleId,
                new UpdateCompanyRoleDto("Manager", [existingPermissionId, addedPermissionId])),
            CancellationToken.None);

        var savedLinks = await unitOfWork.Repository<CompanyRolePermission>()
            .GetAllAsync(CancellationToken.None);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(2, savedLinks.Count);
        Assert.Contains(existingLink, savedLinks);
        Assert.Equal(1, unitOfWork.SaveCount);
    }
}
