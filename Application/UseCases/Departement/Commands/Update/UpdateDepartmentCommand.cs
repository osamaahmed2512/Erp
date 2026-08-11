using Application.Dtos.Departement;
using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Commands.Update
{
    public class UpdateDepartmentCommand : IRequest<BaseApiResponse>
    {
        public Guid DepartmentId { get; set; }
        public UpdateDepartmentDto Dto { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
