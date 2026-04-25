using Application.Dtos.Employee;
using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Employee.Queries.GetById
{
    public record GetEmployeeByIdQuery(Guid Id) :
        IRequest<BaseApiResponse<EmployeeDto>>;
}
