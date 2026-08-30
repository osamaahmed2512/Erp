using Application.Dtos.AccessControl;
using Application.UseCases.AccessControl.Commands.CreateCompanyRole;
using Application.UseCases.AccessControl.Commands.CreateCompanyWithOwner;
using Application.UseCases.AccessControl.Commands.CreateSystemRole;
using Application.UseCases.AccessControl.Commands.CreateSystemUser;
using Application.UseCases.AccessControl.Commands.DeleteCompanyRole;
using Application.UseCases.AccessControl.Commands.DeleteSystemRole;
using Application.UseCases.AccessControl.Commands.SetSystemUserDenies;
using Application.UseCases.AccessControl.Commands.SetSystemUserRoles;
using Application.UseCases.AccessControl.Commands.SetUserOverrides;
using Application.UseCases.AccessControl.Commands.SetUserRoles;
using Application.UseCases.AccessControl.Commands.UpdateCompanyRole;
using Application.UseCases.AccessControl.Commands.UpdateSystemRole;
using Application.UseCases.AccessControl.Queries.GetAccessUsers;
using Application.UseCases.AccessControl.Queries.GetCompanyModuleCatalog;
using Application.UseCases.AccessControl.Queries.GetCompanyRoles;
using Application.UseCases.AccessControl.Queries.GetCurrentAccess;
using Application.UseCases.AccessControl.Queries.GetPermissionCatalog;
using Application.UseCases.AccessControl.Queries.GetSystemRoles;
using Application.UseCases.AccessControl.Queries.GetSystemUserAccess;
using Application.UseCases.AccessControl.Queries.GetSystemUsers;
using Application.UseCases.AccessControl.Queries.GetUserAccess;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrModule.Controllers;

[Route("api/access-control")]
[ApiController]
[Authorize]
public sealed class AccessControlController : BaseController
{
    private readonly IMediator _mediator;
    public AccessControlController(IMediator mediator) => _mediator = mediator;

    [HttpGet("me")]
    public async Task<IActionResult> Me([FromQuery] Guid? companyId, CancellationToken ct) =>
        Ok(await _mediator.Send(new GetCurrentAccessQuery(UserId!.Value, companyId), ct));

    [HttpGet("companies/{companyId:guid}/pages")]
    [Authorize(Policy = Permissions.AccessControl_View)]
    public async Task<IActionResult> CompanyPages(Guid companyId, CancellationToken ct) =>
        Ok(await _mediator.Send(new GetPermissionCatalogQuery(UserId!.Value, companyId), ct));

    [HttpGet("companies/{companyId:guid}/roles")]
    [Authorize(Policy = Permissions.AccessControl_View)]
    public async Task<IActionResult> CompanyRoles(Guid companyId, CancellationToken ct) =>
        Ok(await _mediator.Send(new GetCompanyRolesQuery(UserId!.Value, companyId), ct));

    [HttpPost("companies/{companyId:guid}/roles")]
    [Authorize(Policy = Permissions.AccessControl_Create)]
    public async Task<IActionResult> CreateCompanyRole(Guid companyId, CreateCompanyRoleDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateCompanyRoleCommand(UserId!.Value, companyId, dto), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("companies/{companyId:guid}/roles/{roleId:guid}")]
    [Authorize(Policy = Permissions.AccessControl_Edit)]
    public async Task<IActionResult> UpdateCompanyRole(Guid companyId, Guid roleId, UpdateCompanyRoleDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateCompanyRoleCommand(UserId!.Value, companyId, roleId, dto), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("companies/{companyId:guid}/roles/{roleId:guid}")]
    [Authorize(Policy = Permissions.AccessControl_Delete)]
    public async Task<IActionResult> DeleteCompanyRole(Guid companyId, Guid roleId, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteCompanyRoleCommand(UserId!.Value, companyId, roleId), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("companies/{companyId:guid}/users")]
    [Authorize(Policy = Permissions.AccessControl_View)]
    public async Task<IActionResult> CompanyUsers(Guid companyId, [FromQuery] string? search, CancellationToken ct) =>
        Ok(await _mediator.Send(new GetAccessUsersQuery(UserId!.Value, companyId, search), ct));

    [HttpGet("companies/{companyId:guid}/users/{userId:guid}")]
    [Authorize(Policy = Permissions.AccessControl_View)]
    public async Task<IActionResult> CompanyUserAccess(Guid companyId, Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserAccessQuery(UserId!.Value, companyId, userId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("companies/{companyId:guid}/users/{userId:guid}/roles")]
    [Authorize(Policy = Permissions.AccessControl_Edit)]
    public async Task<IActionResult> SetCompanyUserRoles(Guid companyId, Guid userId, SetUserRolesDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new SetUserRolesCommand(UserId!.Value, companyId, userId, dto), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("companies/{companyId:guid}/users/{userId:guid}/denies")]
    [Authorize(Policy = Permissions.AccessControl_Edit)]
    public async Task<IActionResult> SetCompanyUserDenies(Guid companyId, Guid userId, SetUserOverridesDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new SetUserOverridesCommand(UserId!.Value, companyId, userId, dto), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("system/pages")]
    [Authorize(Policy = Permissions.SystemRoles_View)]
    public async Task<IActionResult> SystemPages(CancellationToken ct) =>
        Ok(await _mediator.Send(new GetPermissionCatalogQuery(UserId!.Value, Guid.Empty), ct));

    [HttpGet("system/company-modules")]
    [Authorize(Policy = Permissions.Companies_Create)]
    public async Task<IActionResult> CompanyModules(CancellationToken ct) =>
        Ok(await _mediator.Send(new GetCompanyModuleCatalogQuery(UserId!.Value), ct));

    [HttpGet("system/roles")]
    [Authorize(Policy = Permissions.SystemRoles_View)]
    public async Task<IActionResult> SystemRoles(CancellationToken ct) =>
        Ok(await _mediator.Send(new GetSystemRolesQuery(UserId!.Value), ct));

    [HttpPost("system/roles")]
    [Authorize(Policy = Permissions.SystemRoles_Create)]
    public async Task<IActionResult> CreateSystemRole(CreateSystemRoleDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSystemRoleCommand(UserId!.Value, dto), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("system/roles/{roleId:guid}")]
    [Authorize(Policy = Permissions.SystemRoles_Edit)]
    public async Task<IActionResult> UpdateSystemRole(Guid roleId, UpdateSystemRoleDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateSystemRoleCommand(UserId!.Value, roleId, dto), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("system/roles/{roleId:guid}")]
    [Authorize(Policy = Permissions.SystemRoles_Delete)]
    public async Task<IActionResult> DeleteSystemRole(Guid roleId, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteSystemRoleCommand(UserId!.Value, roleId), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("system/users")]
    [Authorize(Policy = Permissions.SystemUsers_View)]
    public async Task<IActionResult> SystemUsers([FromQuery] string? search, CancellationToken ct) =>
        Ok(await _mediator.Send(new GetSystemUsersQuery(UserId!.Value, search), ct));

    [HttpPost("system/users")]
    [Authorize(Policy = Permissions.SystemUsers_Create)]
    public async Task<IActionResult> CreateSystemUser(CreateSystemUserDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSystemUserCommand(UserId!.Value, dto), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("system/users/{userId:guid}")]
    [Authorize(Policy = Permissions.SystemUsers_View)]
    public async Task<IActionResult> SystemUserAccess(Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSystemUserAccessQuery(UserId!.Value, userId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("system/users/{userId:guid}/roles")]
    [Authorize(Policy = Permissions.SystemUsers_Edit)]
    public async Task<IActionResult> SetSystemUserRoles(Guid userId, SetUserRolesDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new SetSystemUserRolesCommand(UserId!.Value, userId, dto), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("system/users/{userId:guid}/denies")]
    [Authorize(Policy = Permissions.SystemUsers_Edit)]
    public async Task<IActionResult> SetSystemUserDenies(Guid userId, SetUserOverridesDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new SetSystemUserDeniesCommand(UserId!.Value, userId, dto), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("system/companies-with-owner")]
    [Authorize(Policy = Permissions.Companies_Create)]
    public async Task<IActionResult> CreateCompanyWithOwner(CreateCompanyWithOwnerDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateCompanyWithOwnerCommand(UserId!.Value, dto), ct);
        return StatusCode(result.StatusCode, result);
    }
}
