using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.InternalServices
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserRole { get; }
        bool IsAdmin { get; }
        bool IsSuperAdmin { get; }
        bool IsHr { get; }
    }
}
