using Application.Dtos.Company;
using Application.Dtos.Employee;
using Application.UseCases.Company.Commands.Delete;
using Application.UseCases.Company.Commands.Status;
using Application.UseCases.Company.Commands.Update;
using Application.UseCases.Company.Queries.GetAll;
using Application.UseCases.Company.Queries.GetById;
using Application.UseCases.Company.Queries.GetDropDown;

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
    public class CompanyController : BaseController
    {
        private readonly IMediator _mediator;
        public CompanyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [Authorize(Policy = Permissions.Companies_Edit)]
        [HttpPost("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,[FromBody] UpdateCompanyDto dto)
        {
            var result = await _mediator.Send(new CompanyUpdateCommand
            {
                dto = dto,
                ownerId = UserId.Value,
                companyId = id
            });
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Policy = Permissions.Companies_Delete)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCompanyCommand { companyId = id });
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Policy = Permissions.Companies_View)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCompanyByIdQuery { Id = id });
            return StatusCode(result.StatusCode, result);

        }
        [Authorize(Policy = Permissions.Companies_View)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CompanyPaginationParams paginationParams)
        {
            if (!IsSystemUser) paginationParams.CompanyId = UserCompanyId;
            if (IsOwner)
            {
                paginationParams.OwnerId = UserId.Value;
            }
            var result = await _mediator.Send(new GetAllCompaniesQuery(paginationParams));
            return Ok(result);
        }
        [Authorize(Policy = Permissions.Companies_Edit)]
        [HttpGet("{id:guid}/status")]
        public async Task<IActionResult> changeStatus(Guid id ,string status)
        {
            var result = await _mediator.Send(new CompanyUpdateStatuscommand { Id=id,status =status});
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Policy = Permissions.Companies_View)]
        [HttpGet("DropDown")]
        public async Task<IActionResult> DropDown()
        {
            var command = new GetCompanyDropDownCommand
            {
                OwnerId = IsOwner ? UserId : null
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
