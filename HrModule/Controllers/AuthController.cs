using Application.Dtos.Auth;
using Application.UseCases.Auth.Commands;
using Application.UseCases.Auth.Commands.ConfirmEmail;
using Application.UseCases.Auth.Commands.Login;
using Application.UseCases.Auth.Commands.Register;
using Application.UseCases.Auth.Commands.ResendConfirmEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace HrModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _mediator.Send(command);

            return StatusCode(result.StatusCode,result);
        }
        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> Test()
        {

            return Content("test");
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAdminOrOwner(RegisterUserDto dto)
        {
            var result = await _mediator.Send(new RegisterUserCommand(dto));
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpGet("resend-confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ResendConfirmEmailCommand command )
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
