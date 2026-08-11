using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Commands.Status
{
    public class DepartmentUpdateStatusCommand : IRequest<BaseApiResponse>
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
