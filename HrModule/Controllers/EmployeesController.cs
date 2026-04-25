


using Application.Dtos.Employee;

using Application.UseCases.Employee.Commands.Create;
using Application.UseCases.Employee.Commands.Delete;
using Application.UseCases.Employee.Commands.Update;
using Application.UseCases.Employee.Queries.GetAll;
using Application.UseCases.Employee.Queries.GetById;
using Domain.Specification.Params;
using HrModule.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,HR")]
    public class EmployeesController : BaseController
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            var result = await _mediator.Send(new GetAllEmployeesQuery(paginationParams));
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEmployeeByIdQuery(id));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{comapnyId:guid}/company")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeRequestDto request , Guid comapnyId)
        {
            var result = await _mediator.Send(new CreateEmployeeCommand(request , comapnyId));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request)
        {
            var result = await _mediator.Send(new UpdateEmployeeCommand(id, request));
            return StatusCode(result.StatusCode, result);
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteEmployeeCommand(id));
            return StatusCode(result.StatusCode, result);
        }
    }
}