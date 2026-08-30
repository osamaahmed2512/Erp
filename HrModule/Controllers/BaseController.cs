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
        protected bool IsSuperAdmin => string.Equals(User.FindFirstValue("root_super_admin"), "true", StringComparison.OrdinalIgnoreCase);
        protected bool IsSystemUser => string.Equals(User.FindFirstValue("account_type"), "System", StringComparison.OrdinalIgnoreCase);
        protected Guid? UserCompanyId => Guid.TryParse(User.FindFirstValue("company_id"), out var id) ? id : null;
        protected Guid? ActiveCompanyId => Guid.TryParse(Request.Headers["X-Company-Id"].FirstOrDefault(), out var id) ? id : null;
        protected bool TryResolveCompanyId(Guid? requestedCompanyId, out Guid companyId)
        {
            companyId = Guid.Empty;
            var headerCompanyId = ActiveCompanyId;

            if (!IsSystemUser)
            {
                if (!UserCompanyId.HasValue) return false;
                if (requestedCompanyId.HasValue && requestedCompanyId != UserCompanyId) return false;
                if (headerCompanyId.HasValue && headerCompanyId != UserCompanyId) return false;
                companyId = UserCompanyId.Value;
                return true;
            }

            if (requestedCompanyId.HasValue && headerCompanyId.HasValue &&
                requestedCompanyId != headerCompanyId) return false;
            var resolved = headerCompanyId ?? requestedCompanyId;
            if (!resolved.HasValue) return false;
            companyId = resolved.Value;
            return true;
        }
        protected bool IsHr => string.Equals(UserRole, "HR", StringComparison.OrdinalIgnoreCase);
        protected bool IsOwner => string.Equals(UserRole, "Owner", StringComparison.OrdinalIgnoreCase);
        protected bool IsEmployee => string.Equals(UserRole, "Employee", StringComparison.OrdinalIgnoreCase);
    }
}
