using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class RoleExtensions
    {
        public static string ToRoleName(this SystemRoles role)
        {
            return role.ToString();
        }
    }
}
