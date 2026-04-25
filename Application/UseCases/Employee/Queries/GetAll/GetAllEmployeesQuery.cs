using Application.Dtos.Employee;
using Application.Dtos.Pagination;
using Application.Dtos.Response;
using Domain.Entities;
using Domain.Specification.Params;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Employee.Queries.GetAll
{
    public record GetAllEmployeesQuery(PaginationParams Params) :
        IRequest<BaseApiResponse<PaginationDTO<EmployeeResponse>>>;
}
