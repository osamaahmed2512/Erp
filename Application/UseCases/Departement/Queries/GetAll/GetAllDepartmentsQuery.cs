using Application.Dtos.Departement;
using Application.Dtos.Pagination;
using Domain.Specification.Params;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Queries.GetAll
{
    public class GetAllDepartmentsQuery : IRequest<PaginationDTO<DepartmentDto>>
    {
        public DepartmentPaginationParams PaginationParams { get; set; }

        public GetAllDepartmentsQuery(DepartmentPaginationParams paginationParams)
        {
            PaginationParams = paginationParams;
        }
    }
}
