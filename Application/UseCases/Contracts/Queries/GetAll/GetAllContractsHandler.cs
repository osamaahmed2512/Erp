using Application.Dtos.Contract;
using Application.Dtos.Pagination;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Contract;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Contracts.Queries.GetAll
{
    public class GetAllContractsHandler : IRequestHandler<GetAllContractsQuery, PaginationDTO<ContractDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetAllContractsHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<PaginationDTO<ContractDto>> Handle(GetAllContractsQuery request, CancellationToken cancellationToken)
        {
            var spec = new ContractSpecification(request.PaginationParams);
            var countSpec = new ContractCountSpecification(request.PaginationParams);

            var contracts = await _uow.Repository<Domain.Entities.Contract>()
                .GetProjectedAsync(c => new ContractDto
                {
                    Id = c.Id,
                    EmployeeName = c.Employee.User.FirstName + " " + c.Employee.User.LastName,
                    BasicSalary = c.BasicSalary,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Status = c.Status.ToString()
                }, spec);

            var totalCount = await _uow.Repository<Domain.Entities.Contract>().CountWithSpec(countSpec);

            return new PaginationDTO<ContractDto>
            {
                data = contracts,
                TotalCount = totalCount,
                PageIndex = request.PaginationParams.PageIndex,
                PageSize = request.PaginationParams.PageSize
            };
        }
    }
}
