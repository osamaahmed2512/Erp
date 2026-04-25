using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Employee;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Employee.Commands.Delete
{
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;

        public DeleteEmployeeCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<BaseApiResponse> Handle(
            DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(request.Id);

            if (employee is null)
                return BaseApiResponse.Fail(404, "Employee not found.");

            employee.IsDeleted = true;
            await _uow.SaveChangeAsync(cancellationToken);

            return BaseApiResponse.Success(200, "Employee deleted successfully.");
        }
    }
}
