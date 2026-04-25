using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IApplicationUser
    {
        Guid Id { get; }
        string UserName { get; }
        string Email { get; }
        bool EmailConfirmed { get;}
        string PhoneNumber { get; }
        Employee Employee { get; }
    }
}
