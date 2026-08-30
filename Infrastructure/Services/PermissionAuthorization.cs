using Domain.Entities;
using Domain.Enum;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.Services;

public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _db;
    public PermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor, AppDbContext db)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (!Guid.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return;
        var http = _httpContextAccessor.HttpContext;
        if (http is null) return;
        var ct = http.RequestAborted;

        var user = await _db.Users.AsNoTracking().Where(x => x.Id == userId).Select(x => new
        {
            x.AccountType,
            x.CompanyId,
            x.IsRootSuperAdmin
        }).SingleOrDefaultAsync(ct);
        if (user is null) return;
        var audience = await _db.PermissionDefinitions.AsNoTracking()
            .Where(x => x.Key == requirement.Permission && x.IsActive && x.SystemPage.IsActive)
            .Select(x => (PageAudience?)x.SystemPage.Audience).SingleOrDefaultAsync(ct);
        if (!audience.HasValue) return;

        if (user.AccountType == AccountType.Company &&
            audience is not PageAudience.Company and not PageAudience.Both) return;
        if (user.AccountType == AccountType.System &&
            audience is not PageAudience.System and not PageAudience.Both) return;

        if (!TryReadCompanyContext(http, out var requestCompanyId)) return;
        // System-only pages (for example Companies) are not scoped to the active
        // company selector. A persisted X-Company-Id must not block opening or
        // editing a different company from the system dashboard.
        var hasCompanyRoute = http.Request.RouteValues.ContainsKey("companyId") ||
            http.Request.RouteValues.ContainsKey("comapnyId");
        Guid? companyId = audience == PageAudience.System
            ? null
            : user.AccountType == AccountType.Company
                ? user.CompanyId
                : hasCompanyRoute ? requestCompanyId : null;
        if (user.AccountType == AccountType.Company)
        {
            if (!user.CompanyId.HasValue) return;
            if (requestCompanyId.HasValue && requestCompanyId != user.CompanyId) return;
        }
        if (audience == PageAudience.Both && user.AccountType == AccountType.Company && !companyId.HasValue) return;
        if (companyId.HasValue)
        {
            if (!await _db.Companies.AsNoTracking().AnyAsync(x => x.Id == companyId, ct)) return;
            if (!await ResourceBelongsToCompanyAsync(http, companyId.Value)) return;
        }

        if (user.IsRootSuperAdmin)
        {
            context.Succeed(requirement);
            return;
        }
        var denied = await _db.UserPermissionOverrides.AsNoTracking().AnyAsync(x =>
            x.UserId == userId && x.Effect == PermissionEffect.Deny &&
            x.PermissionDefinition.Key == requirement.Permission, ct);
        if (denied) return;
        var granted = user.AccountType == AccountType.System
            ? await _db.SystemUserRoles.AsNoTracking().AnyAsync(x => x.UserId == userId &&
                x.SystemRole.Permissions.Any(p => p.PermissionDefinition.Key == requirement.Permission), ct)
            : await _db.CompanyUserRoles.AsNoTracking().AnyAsync(x => x.UserId == userId &&
                x.CompanyRole.CompanyId == companyId &&
                x.CompanyRole.Permissions.Any(p => p.PermissionDefinition.Key == requirement.Permission), ct);
        if (granted) context.Succeed(requirement);
    }

    private static bool TryReadCompanyContext(HttpContext http, out Guid? companyId)
    {
        companyId = null;
        var header = http.Request.Headers["X-Company-Id"].FirstOrDefault();
        var route = http.Request.RouteValues["companyId"]?.ToString()
            ?? http.Request.RouteValues["comapnyId"]?.ToString();
        Guid? headerId = null;
        Guid? routeId = null;
        if (!string.IsNullOrWhiteSpace(header))
        {
            if (!Guid.TryParse(header, out var parsed)) return false;
            headerId = parsed;
        }
        if (!string.IsNullOrWhiteSpace(route))
        {
            if (!Guid.TryParse(route, out var parsed)) return false;
            routeId = parsed;
        }
        if (headerId.HasValue && routeId.HasValue && headerId != routeId) return false;
        companyId = headerId ?? routeId;
        return true;
    }

    private async Task<bool> ResourceBelongsToCompanyAsync(HttpContext http, Guid companyId)
    {
        if (!Guid.TryParse(http.Request.RouteValues["id"]?.ToString(), out var resourceId)) return true;
        var controller = http.Request.RouteValues["controller"]?.ToString();
        return controller switch
        {
            "Company" => resourceId == companyId,
            "Department" => await _db.Departments.AnyAsync(x => x.Id == resourceId && x.CompanyId == companyId, http.RequestAborted),
            "Position" => await _db.Positions.AnyAsync(x => x.Id == resourceId && x.Department.CompanyId == companyId, http.RequestAborted),
            "WorkingSchedule" => await _db.WorkingSchedules.AnyAsync(x => x.Id == resourceId && x.CompanyId == companyId, http.RequestAborted),
            "Employees" => await _db.Employees.AnyAsync(x => x.Id == resourceId && x.CompanyId == companyId, http.RequestAborted),
            _ => true
        };
    }
}
