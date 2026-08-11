using Application.Dtos.Departement;
using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Commands.Create
{
    public class CreateDepartmentCommand : IRequest<BaseApiResponse>
    {
        public CreateDepartmentDto Dto { get; set; }
        public Guid OwnerId { get; set; }
    }
}
