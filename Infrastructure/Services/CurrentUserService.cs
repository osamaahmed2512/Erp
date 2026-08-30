using Application.Interfaces.InternalServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CurrentUserService:ICurrentUserService
    {
        private readonly IHttpContextAccessor _context;

        public CurrentUserService(IHttpContextAccessor context)
        {
            _context = context;
        }
        public Guid? UserId => Guid.Parse(_context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        public string? UserRole => _context.HttpContext.User.FindFirstValue(ClaimTypes.Role);
        public bool IsAdmin => string.Equals(UserRole, "Admin", StringComparison.OrdinalIgnoreCase);
        public bool IsSuperAdmin => string.Equals(_context.HttpContext?.User.FindFirstValue("root_super_admin"), "true", StringComparison.OrdinalIgnoreCase);
        public bool IsHr => string.Equals(UserRole, "HR", StringComparison.OrdinalIgnoreCase);
    }
}
