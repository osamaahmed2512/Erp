using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HrModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected Guid? UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) ;
        protected string? UserRole => User.FindFirstValue(ClaimTypes.Role);
        protected bool IsAdmin => string.Equals(UserRole, "Admin", StringComparison.OrdinalIgnoreCase);
        protected bool IsHr => string.Equals(UserRole, "HR", StringComparison.OrdinalIgnoreCase);
    }
}
