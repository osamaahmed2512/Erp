using Application.Dtos.Departement;
using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Queries.GetById
{
    public class GetDepartmentByIdHandler : IRequestHandler<GetDepartmentByIdQuery, BaseApiResponse<DepartmentDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseApiResponse<DepartmentDetailsDto>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var department = await _unitOfWork.Repository<Domain.Entities.Department>().GetByIdAsync(request.Id);

            if (department is null)
                return BaseApiResponse<DepartmentDetailsDto>.Fail(404, "Department not found.");

            var spec = new CompanySpecification(department.CompanyId);
            var company = await _unitOfWork.Repository<Domain.Entities.Company>().GetSingleProjectedAsync(c => new
            {
                c.OwnerId,c.Name
            }, spec);

            if (!request.IsSuperAdmin && company!.OwnerId != request.OwnerId)
                return BaseApiResponse<DepartmentDetailsDto>.Fail(403, "You are not allowed to access this department.");

            var data = new DepartmentDetailsDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                Status = department.Status.ToString(),
                CompanyId = department.CompanyId,
                CompanyName = company!.Name,
                TotalEmployees = 0,
                CreatedAt = department.CreatedAt,
                UpdatedAt = department.UpdatedAt
            };

            return new BaseApiResponse<DepartmentDetailsDto>
            {
                Data = data,
                Message = "Department retrieved successfully.",
                StatusCode = 200
            };
        }
    }
}
