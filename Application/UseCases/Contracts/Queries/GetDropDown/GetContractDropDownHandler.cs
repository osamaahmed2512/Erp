using Application.Dtos.DropDown;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Contract;
using Domain.Specification.Params;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Contracts.Queries.GetDropDown
{
    public class GetContractDropDownHandler : IRequestHandler<GetContractDropDownQuery, List<DropDownDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetContractDropDownHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<List<DropDownDto>> Handle(GetContractDropDownQuery request, CancellationToken cancellationToken)
        {
            var parms = new ContractPaginationParams
            {
                EmployeeId = request.EmployeeId,
                Status = ContractStatus.Active,
                PageIndex = 1,
                PageSize = int.MaxValue
            };
            var spec = new ContractSpecification(parms);
            return await _uow.Repository<Domain.Entities.Contract>()
                .GetProjectedAsync(c => new DropDownDto
                {
                    Id = c.Id,
                    Name = $"{c.Employee.User.FirstName} {c.Employee.User.LastName} — {c.BasicSalary:C} ({c.StartDate:yyyy-MM-dd})"
                }, spec);
        }
    }
}
