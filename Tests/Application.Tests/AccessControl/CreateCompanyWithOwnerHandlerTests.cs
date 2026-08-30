using Application.Dtos.AccessControl;
using Application.Tests.Fakes;
using Application.UseCases.AccessControl.Commands.CreateCompanyWithOwner;
using Domain.Entities;
using Domain.Enum;

namespace Application.Tests.AccessControl;

public sealed class CreateCompanyWithOwnerHandlerTests
{
    [Fact]
    public async Task CreatesOnlyOwnerRole_WithSelectedModuleAndAccessControlPermissions()
    {
        var actorId = Guid.NewGuid();
        var hrPermissionId = Guid.NewGuid();
        var accessPermissionId = Guid.NewGuid();
        var hrPage = Page("Departments", "HR Module", hrPermissionId);
        var accessPage = Page("AccessControl", "Administration", accessPermissionId);
        var unitOfWork = new FakeUnitOfWork()
            .Seed(new ApplicationUser { Id = actorId, IsRootSuperAdmin = true, AccountType = AccountType.System })
            .Seed(hrPage, accessPage)
            .Seed(hrPage.Permissions.Single(), accessPage.Permissions.Single());
        var identity = new FakeIdentityService();
        var handler = new CreateCompanyWithOwnerCommandHandler(unitOfWork, identity);

        var response = await handler.Handle(
            new CreateCompanyWithOwnerCommand(actorId, Request(["HR Module"])),
            CancellationToken.None);

        var roles = await unitOfWork.Repository<CompanyRole>().GetAllAsync();
        var assignments = await unitOfWork.Repository<CompanyUserRole>().GetAllAsync();
        Assert.Equal(201, response.StatusCode);
        var ownerRole = Assert.Single(roles);
        Assert.Equal("OWNER", ownerRole.NormalizedName);
        Assert.True(ownerRole.IsSystem);
        Assert.True(new HashSet<Guid>([hrPermissionId, accessPermissionId]).SetEquals(
            ownerRole.Permissions.Select(x => x.PermissionDefinitionId)));
        Assert.Equal(identity.CreatedUserId, Assert.Single(assignments).UserId);
        Assert.Equal(response.Data, identity.AssignedCompanyId);
    }

    [Fact]
    public async Task RejectsUnknownModule_WithoutCreatingCompanyOrOwner()
    {
        var actorId = Guid.NewGuid();
        var page = Page("Departments", "HR Module", Guid.NewGuid());
        var unitOfWork = new FakeUnitOfWork()
            .Seed(new ApplicationUser { Id = actorId, IsRootSuperAdmin = true, AccountType = AccountType.System })
            .Seed(page)
            .Seed(page.Permissions.Single());
        var identity = new FakeIdentityService();
        var handler = new CreateCompanyWithOwnerCommandHandler(unitOfWork, identity);

        var response = await handler.Handle(
            new CreateCompanyWithOwnerCommand(actorId, Request(["Unknown Module"])),
            CancellationToken.None);

        Assert.Equal(400, response.StatusCode);
        Assert.Empty(await unitOfWork.Repository<Domain.Entities.Company>().GetAllAsync());
        Assert.Null(identity.AssignedCompanyId);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    private static SystemPage Page(string key, string module, Guid permissionId)
    {
        var page = new SystemPage
        {
            Key = key,
            Name = key,
            Module = module,
            Audience = PageAudience.Both,
            IsActive = true
        };
        page.Permissions.Add(new PermissionDefinition
        {
            Id = permissionId,
            Key = $"{key}.View",
            Action = "View",
            IsActive = true,
            SystemPage = page,
            SystemPageId = page.Id
        });
        return page;
    }

    private static CreateCompanyWithOwnerDto Request(IReadOnlyList<string> modules) => new(
        "Acme", "company@acme.test", "01000000000", null, null, null, null, null, null,
        null, null, "owner@acme.test", "Pass123!", "Acme", "Owner", null, modules);
}
