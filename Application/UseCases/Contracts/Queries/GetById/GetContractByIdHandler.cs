using Application.Dtos.Contract;
using Application.Dtos.Response;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Contracts.Queries.GetById
{
    public class GetContractByIdHandler : IRequestHandler<GetContractByIdQuery, BaseApiResponse<ContractDetailsDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetContractByIdHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse<ContractDetailsDto>> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
        {
            var contract = await _uow.Repository<Domain.Entities.Contract>().GetByIdAsync(request.Id);
            if (contract is null || contract.Status == ContractStatus.Terminated)
                return BaseApiResponse<ContractDetailsDto>.Fail(404, "Contract not found.");

            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(contract.EmployeeId);
            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(employee.CompanyId);

            if (!request.IsAdmin && company.OwnerId != request.OwnerId)
                return BaseApiResponse<ContractDetailsDto>.Fail(403, "You are not allowed to access this contract.");

            var data = new ContractDetailsDto
            {
                Id = contract.Id,
                EmployeeId = contract.EmployeeId,
                EmployeeName = employee.User.FirstName + " " + employee.User.LastName,
                BasicSalary = contract.BasicSalary,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status.ToString(),
                CreatedAt = contract.CreatedAt,
                UpdatedAt = contract.UpdatedAt
            };

            return new BaseApiResponse<ContractDetailsDto>
            {
                Data = data,
                Message = "Contract retrieved successfully.",
                StatusCode = 200
            };
        }
    }
}
