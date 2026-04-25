using Application.Dtos.Employee;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.Employee.Commands.Create
{
    public record CreateEmployeeCommand(CreateEmployeeRequestDto Dto, Guid CompanyId) :
        IRequest<BaseApiResponse>;
}
