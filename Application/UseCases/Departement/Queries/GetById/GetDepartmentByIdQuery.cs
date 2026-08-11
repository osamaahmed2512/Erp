using Application.Dtos.Departement;
using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Queries.GetById
{
    public class GetDepartmentByIdQuery : IRequest<BaseApiResponse<DepartmentDetailsDto>>
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
