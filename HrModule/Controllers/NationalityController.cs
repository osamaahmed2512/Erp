using Application.UseCases.Nationality.Queries.GetAll;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HrModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalityController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NationalityController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [Authorize(Policy =Permissions.Nationaality_ViewAll)]
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var result = await _mediator.Send(new GetAllNationalitiesQuery());
            return Ok(result);
        }
    }
}
