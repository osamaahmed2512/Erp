


using Application.Dtos.Employee;

using Application.UseCases.Employee.Commands.Create;
using Application.UseCases.Employee.Commands.Delete;
using Application.UseCases.Employee.Commands.Update;
using Application.UseCases.Employee.Commands.UploadProfilePhoto;
using Application.UseCases.Employee.Queries.GetAll;
using Application.UseCases.Employee.Queries.GetById;
using Domain.Specification.Params;
using HrModule.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Common;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : BaseController
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        [Authorize(Policy = Permissions.Employees_View)]
        public async Task<IActionResult> GetAll([FromQuery] EmployeePaginationParams paginationParams)
        {
            if (!IsSystemUser)
                paginationParams.CompanyId = UserCompanyId;
            var result = await _mediator.Send(new GetAllEmployeesQuery(paginationParams));
            return Ok(result);
        }


        [HttpGet("{id:guid}")]
        [Authorize(Policy = Permissions.Employees_View)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEmployeeByIdQuery(id));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{comapnyId:guid}/company")]
        [Authorize(Policy = Permissions.Employees_Create)]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeRequestDto request , Guid comapnyId)
        {
            var result = await _mediator.Send(new CreateEmployeeCommand(request , comapnyId));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = Permissions.Employees_Edit)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request)
        {
            var result = await _mediator.Send(new UpdateEmployeeCommand(id, request));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id:guid}/profile-photo")]
        [Authorize(Policy = Permissions.Employees_Edit)]
        public async Task<IActionResult> UploadProfilePhoto(Guid id, IFormFile photo)
        {
            var result = await _mediator.Send(new UploadEmployeeProfilePhotoCommand
            {
                EmployeeId = id,
                Photo = photo
            });
            return StatusCode(result.StatusCode, result);
        }


        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Permissions.Employees_Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteEmployeeCommand(id));
            return StatusCode(result.StatusCode, result);
        }
    }
}
