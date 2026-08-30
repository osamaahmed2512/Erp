using Application.Tests.Fakes;
using Application.UseCases.AccessControl.Queries.GetCurrentAccess;
using Domain.Entities;
using Domain.Enum;

namespace Application.Tests.AccessControl;

public sealed class GetCurrentAccessHandlerTests
{
    [Fact]
    public async Task CompanyOwner_UsesOwnerRolePermissions_InsteadOfFullAccessBypass()
    {
        var ownerId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var allowedPage = Page("Departments", "Departments.View");
        var blockedPage = Page("Employees", "Employees.View");
        var ownerRole = new CompanyRole
        {
            CompanyId = companyId,
            Name = "Owner",
            NormalizedName = "OWNER",
            IsSystem = true,
            Permissions =
            [
                new CompanyRolePermission
                {
                    PermissionDefinitionId = allowedPage.Permissions.Single().Id,
                    PermissionDefinition = allowedPage.Permissions.Single()
                }
            ]
        };
        var owner = new ApplicationUser
        {
            Id = ownerId,
            AccountType = AccountType.Company,
            CompanyId = companyId,
            CompanyRoles = [new CompanyUserRole { CompanyRole = ownerRole, CompanyRoleId = ownerRole.Id }]
        };
        var unitOfWork = new FakeUnitOfWork()
            .Seed(owner)
            .Seed(new Domain.Entities.Company
            {
                Id = companyId,
                OwnerId = ownerId,
                Name = "Acme",
                Email = "company@acme.test",
                Phone = "01000000000"
            })
            .Seed(allowedPage, blockedPage);
        var handler = new GetCurrentAccessQueryHandler(unitOfWork);

        var access = await handler.Handle(
            new GetCurrentAccessQuery(ownerId, null), CancellationToken.None);

        Assert.True(access.IsCompanyOwner);
        Assert.Equal(["Departments.View"], access.PermissionKeys);
        Assert.Equal("Departments", Assert.Single(access.Pages).Key);
    }

    private static SystemPage Page(string key, string permissionKey)
    {
        var page = new SystemPage
        {
            Key = key,
            Name = key,
            Module = "HR Module",
            Category = "Settings",
            Route = $"/{key.ToLowerInvariant()}",
            Audience = PageAudience.Both,
            IsActive = true
        };
        page.Permissions.Add(new PermissionDefinition
        {
            Key = permissionKey,
            Action = "View",
            IsActive = true,
            SystemPage = page,
            SystemPageId = page.Id
        });
        return page;
    }
}
