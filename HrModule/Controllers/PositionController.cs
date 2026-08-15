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

namespace HrModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : BaseController
    {
        private readonly IMediator _mediator;
        public PositionController(IMediator mediator) { _mediator = mediator; }

        [Authorize(Roles = "SuperAdmin,Admin,HR,Owner")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePositionDto dto)
        {
            var result = await _mediator.Send(new CreatePositionCommand
            {
                Dto = dto,
                OwnerId = UserId.Value,
                HasGlobalAccess = IsSuperAdmin || IsAdmin || IsHr
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "SuperAdmin,Admin,HR,Owner")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePositionDto dto)
        {
            var result = await _mediator.Send(new UpdatePositionCommand
            {
                PositionId = id,
                Dto = dto,
                OwnerId = UserId.Value,
                IsAdmin = IsSuperAdmin || IsAdmin || IsHr
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeletePositionCommand { PositionId = id });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "SuperAdmin,Admin,HR,Owner")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPositionByIdQuery
            {
                Id = id,
                OwnerId = UserId.Value,
                IsAdmin = IsSuperAdmin || IsAdmin || IsHr
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "SuperAdmin,Admin,HR,Owner")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PositionPaginationParams paginationParams)
        {
            var result = await _mediator.Send(new GetAllPositionsQuery(paginationParams));
            return Ok(result);
        }

        [Authorize(Roles = "SuperAdmin,Admin,HR,Owner")]
        [HttpGet("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] string status)
        {
            var result = await _mediator.Send(new PositionUpdateStatusCommand
            {
                Id = id,
                Status = status,
                OwnerId = UserId.Value,
                IsAdmin = IsSuperAdmin || IsAdmin || IsHr
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "SuperAdmin,Admin,HR,Owner")]
        [HttpGet("dropdown")]
        public async Task<IActionResult> DropDown([FromQuery] Guid? departmentId)
        {
            var result = await _mediator.Send(new GetPositionDropDownQuery { DepartmentId = departmentId });
            return Ok(result);
        }
    }
}
