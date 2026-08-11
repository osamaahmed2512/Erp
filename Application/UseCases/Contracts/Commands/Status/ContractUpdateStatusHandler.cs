using Application.Dtos.Response;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Contracts.Commands.Status
{
    public class ContractUpdateStatusHandler : IRequestHandler<ContractUpdateStatusCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        public ContractUpdateStatusHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse> Handle(ContractUpdateStatusCommand request, CancellationToken cancellationToken)
        {
            var contract = await _uow.Repository<Domain.Entities.Contract>().GetByIdAsync(request.Id);
            if (contract is null || contract.Status == ContractStatus.Terminated)
                return BaseApiResponse.Fail(404, "Contract not found.");

            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(contract.EmployeeId);
            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(employee.CompanyId);

            if (!request.IsAdmin && company.OwnerId != request.OwnerId)
                return BaseApiResponse.Fail(403, "You are not allowed to change the status of this contract.");

            if (!Enum.TryParse<ContractStatus>(request.Status, true, out var newStatus))
                return BaseApiResponse.Fail(400, "Invalid contract status. Valid values: Active, Expired, Terminated.");

            contract.Status = newStatus;
            contract.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(200, $"Contract status changed to {contract.Status} successfully.");
        }
    }
}
