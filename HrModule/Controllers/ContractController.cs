using Application.Dtos.Contract;
using Application.UseCases.Contracts.Commands.Create;
using Application.UseCases.Contracts.Commands.Delete;
using Application.UseCases.Contracts.Commands.Status;
using Application.UseCases.Contracts.Commands.Update;
using Application.UseCases.Contracts.Queries.GetAll;
using Application.UseCases.Contracts.Queries.GetById;
using Application.UseCases.Contracts.Queries.GetDropDown;
using Domain.Specification.Params;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : BaseController
    {
        private readonly IMediator _mediator;
        public ContractController(IMediator mediator) { _mediator = mediator; }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContractDto dto)
        {
            var result = await _mediator.Send(new CreateContractCommand
            {
                Dto = dto,
                OwnerId = UserId.Value,
                IsAdmin = IsAdmin
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContractDto dto)
        {
            var result = await _mediator.Send(new UpdateContractCommand
            {
                ContractId = id,
                Dto = dto,
                OwnerId = UserId.Value,
                IsAdmin = IsAdmin
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteContractCommand { ContractId = id });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetContractByIdQuery
            {
                Id = id,
                OwnerId = UserId.Value,
                IsAdmin = IsAdmin
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ContractPaginationParams paginationParams)
        {
            var result = await _mediator.Send(new GetAllContractsQuery(paginationParams));
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpGet("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] string status)
        {
            var result = await _mediator.Send(new ContractUpdateStatusCommand
            {
                Id = id,
                Status = status,
                OwnerId = UserId.Value,
                IsAdmin = IsAdmin
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpGet("dropdown")]
        public async Task<IActionResult> DropDown([FromQuery] Guid? employeeId)
        {
            var result = await _mediator.Send(new GetContractDropDownQuery { EmployeeId = employeeId });
            return Ok(result);
        }
    }
}
