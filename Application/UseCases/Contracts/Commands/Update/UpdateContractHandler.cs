using Application.Dtos.Response;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using Domain.Specification.Employee;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Contracts.Commands.Update
{
    public class UpdateContractHandler : IRequestHandler<UpdateContractCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        public UpdateContractHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
        {
            var contract = await _uow.Repository<Domain.Entities.Contract>().GetByIdAsync(request.ContractId);
            if (contract is null || contract.Status == ContractStatus.Terminated)
                return BaseApiResponse.Fail(404, "Contract not found.");

            var employeeSpec = new EmployeeSpecification(contract.EmployeeId);
            var employee = await _uow.Repository<Domain.Entities.Employee>().GetSingleProjectedAsync(e => new
            {
                e.CompanyId
            }, employeeSpec);
            var companySpec = new CompanySpecification(employee.CompanyId);
            var company = await _uow.Repository<Domain.Entities.Company>().GetSingleProjectedAsync(c => new
            {
                c.OwnerId
            }, companySpec);

            if (!request.IsAdmin && company.OwnerId != request.OwnerId)
                return BaseApiResponse.Fail(403, "You are not allowed to update this contract.");

            var dto = request.Dto;

            if (dto.EndDate.HasValue && dto.EndDate <= dto.StartDate)
                return BaseApiResponse.Fail(400, "End date must be after start date.");

            contract.BasicSalary = dto.BasicSalary;
            contract.StartDate = dto.StartDate;
            contract.EndDate = dto.EndDate;
            contract.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(200, "Contract updated successfully.");
        }
    }
}
