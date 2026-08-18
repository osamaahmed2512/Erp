using Application.Dtos.WorkingSchedule;
using Application.UseCases.WorkingSchedule.Commands.Create;
using Application.UseCases.WorkingSchedule.Commands.Delete;
using Application.UseCases.WorkingSchedule.Commands.Status;
using Application.UseCases.WorkingSchedule.Commands.Update;
using Application.UseCases.WorkingSchedule.Queries.GetAll;
using Application.UseCases.WorkingSchedule.Queries.GetById;
using Application.UseCases.WorkingSchedule.Queries.GetDropDown;
using Domain.Common;
using Domain.Specification.Params;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrModule.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkingScheduleController : BaseController
{
    private readonly IMediator _mediator;
    public WorkingScheduleController(IMediator mediator) => _mediator = mediator;

    [Authorize(Policy = Permissions.WorkingSchedules_Add)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkingScheduleDto dto)
    {
        var result = await _mediator.Send(new CreateWorkingScheduleCommand(dto, UserId!.Value, IsSuperAdmin));
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Policy = Permissions.WorkingSchedules_ViewAll)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] WorkingSchedulePaginationParams parameters) =>
        Ok(await _mediator.Send(new GetWorkingSchedulesQuery(parameters, UserId!.Value, IsSuperAdmin)));

    [Authorize(Policy = Permissions.WorkingSchedules_View)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetWorkingScheduleByIdQuery(id, UserId!.Value, IsSuperAdmin));
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Policy = Permissions.WorkingSchedules_Edit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkingScheduleDto dto)
    {
        var result = await _mediator.Send(new UpdateWorkingScheduleCommand(id, dto, UserId!.Value, IsSuperAdmin));
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Policy = Permissions.WorkingSchedules_Edit)]
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] bool isActive)
    {
        var result = await _mediator.Send(new ChangeWorkingScheduleStatusCommand(id, isActive, UserId!.Value, IsSuperAdmin));
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Policy = Permissions.WorkingSchedules_Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteWorkingScheduleCommand(id, UserId!.Value, IsSuperAdmin));
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "SuperAdmin,Admin,HR,Owner")]
    [HttpGet("dropdown")]
    public async Task<IActionResult> DropDown([FromQuery] Guid? companyId) =>
        Ok(await _mediator.Send(new GetWorkingScheduleDropDownQuery(companyId, UserId!.Value, IsSuperAdmin)));
}
