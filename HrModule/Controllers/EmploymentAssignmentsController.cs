using Application.Dtos.EmploymentAssignment;
using Application.UseCases.EmploymentAssignments.Commands.Create;
using Application.UseCases.EmploymentAssignments.Queries.GetCurrent;
using Application.UseCases.EmploymentAssignments.Queries.GetHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrModule.Controllers
{
    [Route("api/employees/{employeeId:guid}/assignments")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Admin,HR")]
    public class EmploymentAssignmentsController : BaseController
    {
        private readonly IMediator _mediator;

        public EmploymentAssignmentsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create(
            Guid employeeId,
            [FromBody] CreateEmploymentAssignmentDto dto)
        {
            var result = await _mediator.Send(new CreateEmploymentAssignmentCommand
            {
                EmployeeId = employeeId,
                Dto = dto,
                CurrentUserId = UserId!.Value,
                HasGlobalAccess = IsSuperAdmin || IsAdmin || IsHr
            });

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory(Guid employeeId)
        {
            var result = await _mediator.Send(new GetEmploymentAssignmentHistoryQuery
            {
                EmployeeId = employeeId,
                CurrentUserId = UserId!.Value,
                HasGlobalAccess = IsSuperAdmin || IsAdmin || IsHr
            });

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(
            Guid employeeId,
            [FromQuery] DateOnly? effectiveDate)
        {
            var result = await _mediator.Send(new GetCurrentEmploymentAssignmentQuery
            {
                EmployeeId = employeeId,
                EffectiveDate = effectiveDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                CurrentUserId = UserId!.Value,
                HasGlobalAccess = IsSuperAdmin || IsAdmin || IsHr
            });

            return StatusCode(result.StatusCode, result);
        }
    }
}
