using Application.Dtos.Response;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using Domain.Specification.Contract;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Contracts.Commands.Create
{
    public class CreateContractHandler : IRequestHandler<CreateContractCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        public CreateContractHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse> Handle(CreateContractCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(dto.EmployeeId);
            if (employee is null || employee.IsDeleted)
                return BaseApiResponse.Fail(404, "Employee not found.");

            var companySpec = new CompanySpecification(employee.CompanyId);
            var company = await _uow.Repository<Domain.Entities.Company>().GetSingleProjectedAsync(c => new
            {
                c.OwnerId
            }, companySpec);
            if (!request.IsAdmin && company!.OwnerId != request.OwnerId)
                return BaseApiResponse.Fail(403, "You are not allowed to create contracts for this employee.");

            // Business rule: only one Active contract per employee
            var activeSpec = new ContractSpecification(dto.EmployeeId, ContractStatus.Active);
            var activeContract = await _uow.Repository<Domain.Entities.Contract>()
                .GetSingleProjectedAsync(x => new { x.Id }, activeSpec);

            if (activeContract is not null)
                return BaseApiResponse.Fail(400, "Employee already has an active contract. Terminate it before creating a new one.");

            if (dto.EndDate.HasValue && dto.EndDate <= dto.StartDate)
                return BaseApiResponse.Fail(400, "End date must be after start date.");

            var contract = new Domain.Entities.Contract
            {
                EmployeeId = dto.EmployeeId,
                BasicSalary = dto.BasicSalary,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = ContractStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Repository<Domain.Entities.Contract>().AddAsync(contract);
            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(201, "Contract created successfully.");
        }
    }
}
