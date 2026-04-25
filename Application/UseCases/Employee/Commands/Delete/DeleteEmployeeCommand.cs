using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Employee.Commands.Delete
{
    public record DeleteEmployeeCommand(Guid Id) : IRequest<BaseApiResponse>;
}
