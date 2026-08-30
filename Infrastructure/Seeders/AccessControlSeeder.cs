using Domain.Common;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeders;

public static class AccessControlSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        foreach (var definition in PermissionCatalog.Pages)
        {
            var page = await db.SystemPages.Include(x => x.Permissions)
                .SingleOrDefaultAsync(x => x.Key == definition.Key, ct);
            if (page is null)
            {
                page = new SystemPage { Key = definition.Key };
                db.SystemPages.Add(page);
            }
            page.Name = definition.Name;
            page.Module = definition.Module;
            page.Category = definition.Category;
            page.Route = definition.Route;
            page.Icon = definition.Icon;
            page.DisplayOrder = definition.DisplayOrder;
            page.Audience = definition.Audience;
            page.IsActive = true;
            foreach (var action in definition.Actions)
            {
                var key = $"{definition.Key}.{action}";
                var permission = page.Permissions.SingleOrDefault(x => x.Key == key);
                if (permission is null)
                {
                    permission = new PermissionDefinition { Key = key, SystemPage = page };
                    page.Permissions.Add(permission);
                }
                permission.Action = action;
                permission.IsActive = true;
            }
        }
        await db.SaveChangesAsync(ct);

        if (!await db.SystemRoles.AnyAsync(x => x.NormalizedName == "SUPPORT ADMIN", ct))
            db.SystemRoles.Add(new SystemRole { Name = "Support Admin", NormalizedName = "SUPPORT ADMIN" });

        var allCompanyPermissionIds = await db.PermissionDefinitions.AsNoTracking()
            .Where(x => x.IsActive && x.SystemPage.IsActive &&
                (x.SystemPage.Audience == PageAudience.Company || x.SystemPage.Audience == PageAudience.Both))
            .Select(x => x.Id)
            .ToListAsync(ct);
        var companies = await db.Companies.AsNoTracking().Select(x => new { x.Id, x.OwnerId }).ToListAsync(ct);
        foreach (var company in companies)
        {
            var ownerRole = await db.CompanyRoles.Include(x => x.Permissions).SingleOrDefaultAsync(x =>
                x.CompanyId == company.Id && x.NormalizedName == "OWNER", ct);
            if (ownerRole is null)
            {
                ownerRole = new CompanyRole
                {
                    CompanyId = company.Id,
                    Name = "Owner",
                    NormalizedName = "OWNER",
                    IsSystem = true
                };
                db.CompanyRoles.Add(ownerRole);
            }
            if (ownerRole.Permissions.Count == 0)
                ownerRole.Permissions = allCompanyPermissionIds.Select(permissionId =>
                    new CompanyRolePermission { PermissionDefinitionId = permissionId }).ToList();

            var owner = await db.Users.SingleOrDefaultAsync(x => x.Id == company.OwnerId, ct);
            if (owner is not null)
            {
                owner.AccountType = AccountType.Company;
                owner.CompanyId = company.Id;
                if (!await db.CompanyUserRoles.AnyAsync(x =>
                        x.UserId == owner.Id && x.CompanyRoleId == ownerRole.Id, ct))
                    db.CompanyUserRoles.Add(new CompanyUserRole
                    {
                        UserId = owner.Id,
                        CompanyRoleId = ownerRole.Id
                    });
            }
        }
        await db.SaveChangesAsync(ct);

        var employees = await db.Employees.AsNoTracking().Where(x => x.UserId != Guid.Empty)
            .Select(x => new { x.UserId, x.CompanyId }).ToListAsync(ct);
        foreach (var employee in employees)
        {
            var user = await db.Users.SingleOrDefaultAsync(x => x.Id == employee.UserId, ct);
            if (user is null) continue;
            user.AccountType = AccountType.Company;
            user.CompanyId = employee.CompanyId;
            var role = await db.CompanyRoles.SingleOrDefaultAsync(x =>
                x.CompanyId == employee.CompanyId && x.NormalizedName == "EMPLOYEE", ct);
            if (role is null) continue;
            if (!await db.CompanyUserRoles.AnyAsync(x =>
                    x.UserId == employee.UserId && x.CompanyRoleId == role.Id, ct))
                db.CompanyUserRoles.Add(new CompanyUserRole { UserId = employee.UserId, CompanyRoleId = role.Id });
        }
        await db.SaveChangesAsync(ct);
    }
}
