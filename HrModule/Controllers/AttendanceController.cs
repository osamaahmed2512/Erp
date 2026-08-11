using Application.UseCases.Attendance.Commands.CheckIn;
using Application.UseCases.Attendance.Commands.CheckOut;
using Application.UseCases.Attendance.Queries.GetAll;
using Application.UseCases.Attendance.Queries.GetById;
using Application.UseCases.Attendance.Queries.GetSummary;
using Domain.Attendence;
using Domain.Entities;
using Domain.Specification.Params;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : BaseController
    {
        private readonly IMediator _mediator;
        public AttendanceController(IMediator mediator) { _mediator = mediator; }

        [Authorize(Roles = "Admin,Owner,HR,Manager,Employee")]
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
        {
            var result = await _mediator.Send(new CheckInCommand
            {
                Dto = dto,
                CurrentUserId = UserId.Value,
                CurrentUserRole = UserRole
            });
            return StatusCode(result.StatusCode, result);
        }

        // All roles can check out
        [Authorize(Roles = "Admin,Owner,HR,Manager,Employee")]
        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutDto dto)
        {
            var result = await _mediator.Send(new CheckOutCommand
            {
                Dto = dto,
                CurrentUserId = UserId.Value,
                CurrentUserRole = UserRole
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin,Owner,HR,Manager,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AttendancePaginationParams paginationParams)
        {
            // Employee self-service: force filter to their own records
            if (IsEmployee)
                paginationParams.EmployeeId = (UserId);

            var result = await _mediator.Send(new GetAllAttendanceQuery(paginationParams));
            return Ok(result);
        }

        // Any role can view a record (scoped inside handler)
        [Authorize(Roles = "Admin,Owner,HR,Manager,Employee")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetAttendanceByIdQuery
            {
                Id = id,
                CurrentUserId = UserId.Value,
                CurrentUserRole = UserRole
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin,Owner,HR,Manager,Employee")]
        [HttpGet("summary/{employeeId:guid}")]
        public async Task<IActionResult> GetSummary(Guid employeeId, [FromQuery] int month, [FromQuery] int year)
        {
            var result = await _mediator.Send(new GetAttendanceSummaryQuery
            {
                EmployeeId = employeeId,
                Month = month,
                Year = year,
                CurrentUserId = UserId.Value,
                CurrentUserRole = UserRole
            });
            return StatusCode(result.StatusCode, result);
        }

    }
}
