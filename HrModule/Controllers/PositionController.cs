using Application.Dtos.Position;
using Application.UseCases.Position.Commands.Create;
using Application.UseCases.Position.Commands.Delete;
using Application.UseCases.Position.Commands.Status;
using Application.UseCases.Position.Commands.Update;
using Application.UseCases.Position.Queries.GetAll;
using Application.UseCases.Position.Queries.GetById;
using Application.UseCases.Position.Queries.GetDropDown;
using Domain.Specification.Params;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Common;

namespace HrModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : BaseController
    {
        private readonly IMediator _mediator;
        public PositionController(IMediator mediator) { _mediator = mediator; }

        [Authorize(Policy = Permissions.Positions_Create)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePositionDto dto)
        {
            if (!ActiveCompanyId.HasValue) return BadRequest("An active company is required.");
            var result = await _mediator.Send(new CreatePositionCommand
            {
                Dto = dto,
                OwnerId = UserId.Value,
                HasGlobalAccess = true,
                CompanyId = ActiveCompanyId.Value
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = Permissions.Positions_Edit)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePositionDto dto)
        {
            if (!ActiveCompanyId.HasValue) return BadRequest("An active company is required.");
            var result = await _mediator.Send(new UpdatePositionCommand
            {
                PositionId = id,
                Dto = dto,
                OwnerId = UserId.Value,
                IsAdmin = true,
                CompanyId = ActiveCompanyId.Value
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = Permissions.Positions_Delete)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeletePositionCommand { PositionId = id });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = Permissions.Positions_View)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPositionByIdQuery
            {
                Id = id,
                OwnerId = UserId.Value,
                IsAdmin = true
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = Permissions.Positions_View)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PositionPaginationParams paginationParams)
        {
            if (!IsSystemUser)
                paginationParams.CompanyId = UserCompanyId;
            var result = await _mediator.Send(new GetAllPositionsQuery(paginationParams));
            return Ok(result);
        }

        [Authorize(Policy = Permissions.Positions_Edit)]
        [HttpGet("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] string status)
        {
            var result = await _mediator.Send(new PositionUpdateStatusCommand
            {
                Id = id,
                Status = status,
                OwnerId = UserId.Value,
                IsAdmin = true
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = Permissions.Positions_View)]
        [HttpGet("dropdown")]
        public async Task<IActionResult> DropDown([FromQuery] Guid? departmentId)
        {
            var result = await _mediator.Send(new GetPositionDropDownQuery { DepartmentId = departmentId });
            return Ok(result);
        }
    }
}
