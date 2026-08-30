using Domain.Common;
using Domain.Enum;
using Xunit;

namespace Domain.Tests;

public sealed class PermissionTests
{
    [Fact]
    public void Resolve_UnionsRoles_AndAppliesDenies()
    {
        var result = EffectivePermissionResolver.Resolve(
            ["Employees.View", "Employees.Edit", "Departments.View"],
            ["Employees.Edit"]);

        Assert.Contains("Employees.View", result);
        Assert.DoesNotContain("Employees.Edit", result);
    }

    [Fact]
    public void Catalog_HasUniquePageAndPermissionKeys()
    {
        Assert.Equal(PermissionCatalog.Pages.Count, PermissionCatalog.Pages.Select(x => x.Key).Distinct().Count());
        var keys = Permissions.GetAll();
        Assert.Equal(keys.Count, keys.Distinct().Count());
        Assert.Contains(Permissions.AccessControl_View, keys);
    }
}
