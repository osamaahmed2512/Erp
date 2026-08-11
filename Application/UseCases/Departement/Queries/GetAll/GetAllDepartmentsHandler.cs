using Application.Dtos.Departement;
using Application.Dtos.Pagination;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Departement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Queries.GetAll
{
    public class GetAllDepartmentsHandler : IRequestHandler<GetAllDepartmentsQuery, PaginationDTO<DepartmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllDepartmentsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginationDTO<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var spec = new DepartementPaginationSpecification(request.PaginationParams);
            var countSpec = new DepartmentCountSpecification(request.PaginationParams);

            var departments = await _unitOfWork.Repository<Domain.Entities.Department>()
                .GetProjectedAsync(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Status = d.Status.ToString(),
                    CompanyName = d.Company.Name,
                    TotalEmployees = 0
                }, spec);

            var totalCount = await _unitOfWork.Repository<Domain.Entities.Department>()
                .CountWithSpec(countSpec);

            return new PaginationDTO<DepartmentDto>
            {
                data = departments,
                TotalCount = totalCount,
                PageIndex = request.PaginationParams.PageIndex,
                PageSize = request.PaginationParams.PageSize
            };
        }
    }

}
