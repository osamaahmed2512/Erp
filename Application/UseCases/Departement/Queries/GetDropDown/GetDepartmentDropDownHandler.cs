using Application.Dtos.DropDown;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Departement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Queries.GetDropDown
{
    public class GetDepartmentDropDownHandler : IRequestHandler<GetDepartmentDropDownQuery, List<DropDownDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentDropDownHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DropDownDto>> Handle(GetDepartmentDropDownQuery request, CancellationToken cancellationToken)
        {
            var paginationParams = new Domain.Specification.Params.DepartmentPaginationParams
            {
                CompanyId = request.CompanyId
            };

            var spec = new DepartmentSpecification(paginationParams);

            var departments = await _unitOfWork.Repository<Domain.Entities.Department>()
                .GetProjectedAsync(d => new DropDownDto
                {
                    Id = d.Id,
                    Name = d.Name
                }, spec);

            return departments;
        }
    }
}
