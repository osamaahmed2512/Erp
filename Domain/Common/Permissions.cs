using Domain.Enum;

namespace Domain.Common;

public static class Permissions
{
    public const string AccessControl_View = "AccessControl.View";
    public const string AccessControl_Create = "AccessControl.Create";
    public const string AccessControl_Edit = "AccessControl.Edit";
    public const string AccessControl_Delete = "AccessControl.Delete";
    public const string Companies_View = "Companies.View";
    public const string Companies_Create = "Companies.Create";
    public const string Companies_Edit = "Companies.Edit";
    public const string Companies_Delete = "Companies.Delete";
    public const string Departments_View = "Departments.View";
    public const string Departments_Create = "Departments.Create";
    public const string Departments_Edit = "Departments.Edit";
    public const string Departments_Delete = "Departments.Delete";
    public const string Positions_View = "Positions.View";
    public const string Positions_Create = "Positions.Create";
    public const string Positions_Edit = "Positions.Edit";
    public const string Positions_Delete = "Positions.Delete";
    public const string WorkingSchedules_View = "WorkingSchedules.View";
    public const string WorkingSchedules_Create = "WorkingSchedules.Create";
    public const string WorkingSchedules_Edit = "WorkingSchedules.Edit";
    public const string WorkingSchedules_Delete = "WorkingSchedules.Delete";
    public const string Employees_View = "Employees.View";
    public const string Employees_Create = "Employees.Create";
    public const string Employees_Edit = "Employees.Edit";
    public const string Employees_Delete = "Employees.Delete";
    public const string SystemUsers_View = "SystemUsers.View";
    public const string SystemUsers_Create = "SystemUsers.Create";
    public const string SystemUsers_Edit = "SystemUsers.Edit";
    public const string SystemUsers_Delete = "SystemUsers.Delete";
    public const string SystemRoles_View = "SystemRoles.View";
    public const string SystemRoles_Create = "SystemRoles.Create";
    public const string SystemRoles_Edit = "SystemRoles.Edit";
    public const string SystemRoles_Delete = "SystemRoles.Delete";

    public const string Companies_Add = Companies_Create;
    public const string Departement_View = Departments_View;
    public const string Departement_ViewAll = Departments_View;
    public const string Departement_Add = Departments_Create;
    public const string Departement_Edit = Departments_Edit;
    public const string Departement_Delete = Departments_Delete;
    public const string WorkingSchedules_ViewAll = WorkingSchedules_View;
    public const string WorkingSchedules_Add = WorkingSchedules_Create;
    public const string Nationaality_ViewAll = Employees_View;

    public static IReadOnlyList<string> GetAll() => PermissionCatalog.Pages
        .SelectMany(page => page.Actions.Select(action => $"{page.Key}.{action}"))
        .Concat([Nationaality_ViewAll])
        .Distinct(StringComparer.Ordinal)
        .ToArray();
}

public sealed record PermissionPageSeed(
    string Key, string Name, string Module, string Category, string Route,
    string Icon, int DisplayOrder, PageAudience Audience, params string[] Actions);

public static class PermissionCatalog
{
    private static readonly string[] Crud = ["View", "Create", "Edit", "Delete"];

    public static IReadOnlyList<PermissionPageSeed> Pages { get; } =
    [
        new("Companies", "Companies", "System", "Administration", "/company", "building-2", 10, PageAudience.System, Crud),
        new("Departments", "Departments", "HR Module", "Settings", "/departement", "building-2", 20, PageAudience.Both, Crud),
        new("Positions", "Positions", "HR Module", "Settings", "/position", "briefcase", 30, PageAudience.Both, Crud),
        new("WorkingSchedules", "Working Schedules", "HR Module", "Settings", "/working-schedule", "clock", 40, PageAudience.Both, Crud),
        new("Employees", "Employees", "HR Module", "Employees", "/employee", "users", 50, PageAudience.Both, Crud),
        new("AccessControl", "Company Access Control", "Administration", "Company", "/admin/access-control", "shield-check", 60, PageAudience.Both, Crud),
        new("SystemUsers", "System Users", "System", "Administration", "/admin/system-access-control", "users", 70, PageAudience.System, Crud),
        new("SystemRoles", "System Roles", "System", "Administration", "/admin/system-access-control", "shield-check", 80, PageAudience.System, Crud)
    ];
}
