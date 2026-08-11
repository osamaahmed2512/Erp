using Application.Dtos.Departement;
using Application.UseCases.Departement.Commands.Create;
using Application.UseCases.Departement.Commands.Delete;
using Application.UseCases.Departement.Commands.Status;
using Application.UseCases.Departement.Commands.Update;
using Application.UseCases.Departement.Queries.GetAll;
using Application.UseCases.Departement.Queries.GetById;
using Application.UseCases.Departement.Queries.GetDropDown;
using Domain.Common;
using Domain.Specification.Params;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : BaseController
    {
        private readonly IMediator _mediator;
        public DepartmentController(IMediator mediator) { _mediator = mediator; }

        [Authorize(Policy = Permissions.Departement_Add)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
        {
            var result = await _mediator.Send(new CreateDepartmentCommand { Dto = dto, OwnerId = UserId!.Value });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = Permissions.Departement_Edit)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentDto dto)
        {
            var result = await _mediator.Send(new UpdateDepartmentCommand
            {
                DepartmentId = id,
                Dto = dto,
                OwnerId = UserId.Value,
                IsSuperAdmin = IsSuperAdmin
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = Permissions.Departement_Delete)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteDepartmentCommand { DepartmentId = id });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = Permissions.Departement_View)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetDepartmentByIdQuery
            {
                Id = id,
                OwnerId = UserId.Value,
                IsSuperAdmin = IsSuperAdmin
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = Permissions.Departement_ViewAll)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DepartmentPaginationParams paginationParams)
        {
            var result = await _mediator.Send(new GetAllDepartmentsQuery(paginationParams));
            return Ok(result);
        }

        [Authorize(Policy = Permissions.Departement_ViewAll)]
        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id)
        {
            var result = await _mediator.Send(new DepartmentUpdateStatusCommand
            {
                Id = id,
                OwnerId = UserId.Value,
                IsSuperAdmin = IsSuperAdmin
            });
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "SuperAdmin,Admin,HR,Owner")]
        [HttpGet("dropdown")]
        public async Task<IActionResult> DropDown([FromQuery] Guid? companyId)
        {
            var result = await _mediator.Send(new GetDepartmentDropDownQuery { CompanyId = companyId });
            return Ok(result);
        }
    }
}
