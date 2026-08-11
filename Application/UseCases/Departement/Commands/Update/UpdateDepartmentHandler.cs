using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using Domain.Specification.Departement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Commands.Update
{
    public class UpdateDepartmentHandler : IRequestHandler<UpdateDepartmentCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseApiResponse> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _unitOfWork.Repository<Domain.Entities.Department>().GetByIdAsync(request.DepartmentId);

            if (department is null)
                return BaseApiResponse.Fail(404, "Department not found.");

            var companySpec = new CompanySpecification(department.CompanyId);
            var company = await _unitOfWork.Repository<Domain.Entities.Company>().GetSingleProjectedAsync(c => new
            {
                c.OwnerId
            }, companySpec);

            if (!request.IsSuperAdmin && company!.OwnerId != request.OwnerId)
                return BaseApiResponse.Fail(403, "You are not allowed to update this department.");

            // Check name uniqueness excluding current department
            var spec = new DepartmentSpecification(request.Dto.Name.Trim(), department.CompanyId, department.Id);
            var existing = await _unitOfWork.Repository<Domain.Entities.Department>()
                .GetSingleProjectedAsync(x => new { x.Name }, spec);

            if (existing is not null)
                return BaseApiResponse.Fail(400, "A department with this name already exists in the company.");

            department.Name = request.Dto.Name.Trim();
            department.Description = request.Dto.Description?.Trim();
            department.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangeAsync(cancellationToken);

            return BaseApiResponse.Success(200, "Department updated successfully.");
        }
    }
}
