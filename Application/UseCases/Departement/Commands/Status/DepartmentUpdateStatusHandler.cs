using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Commands.Status
{
    public class DepartmentUpdateStatusHandler : IRequestHandler<DepartmentUpdateStatusCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentUpdateStatusHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseApiResponse> Handle(DepartmentUpdateStatusCommand request, CancellationToken cancellationToken)
        {
            var department = await _unitOfWork.Repository<Domain.Entities.Department>().GetByIdAsync(request.Id);

            if (department is null)
                return BaseApiResponse.Fail(404, "Department not found.");
            var spec = new CompanySpecification(department.CompanyId);
            var company = await _unitOfWork.Repository<Domain.Entities.Company>().GetSingleProjectedAsync(c => new
            {
                c.OwnerId
            },spec);

            if (!request.IsSuperAdmin && company!.OwnerId != request.OwnerId)
                return BaseApiResponse.Fail(403, "You are not allowed to change the status of this department.");


            department.Status = department.Status.ToString() == "Active"
                ? Domain.Enum.EntityStatus.Inactive
                : Domain.Enum.EntityStatus.Active;

            department.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangeAsync(cancellationToken);

            return BaseApiResponse.Success(200, $"Department status changed to {department.Status} successfully.");
        }
    }
}
