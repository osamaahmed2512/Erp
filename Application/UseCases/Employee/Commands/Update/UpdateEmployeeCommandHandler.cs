using Application.Dtos.Response;
using Domain.Entities;
using Domain.Interfaces.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Employee.Commands.Update
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateEmployeeCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<BaseApiResponse> Handle(
            UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(request.Id);

            if (employee is null)
                return BaseApiResponse.Fail(404, "Employee not found.");

            employee.User.FirstName = request.Request.FirstName;
            employee.User.LastName = request.Request.LastName;

            await _uow.Repository<Domain.Entities.Employee>().UpdateAsync(employee);
            await _uow.SaveChangeAsync(cancellationToken);

            return BaseApiResponse.Success(200, "Employee updated successfully.");
        }
    }
}
