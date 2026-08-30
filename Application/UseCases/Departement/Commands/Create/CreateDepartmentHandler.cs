using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Departement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Commands.Create
{
    public class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateDepartmentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseApiResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (!dto.CompanyId.HasValue)
                return BaseApiResponse.Fail(400, "A valid company context is required.");
            var companyId = dto.CompanyId.Value;

            var company = await _unitOfWork.Repository<Domain.Entities.Company>()
                .GetByIdAsync(companyId, cancellationToken);
            if (company is null)
                return BaseApiResponse.Fail(404, "Company not found.");

            var spec = new DepartmentSpecification(dto.Name.Trim(), companyId);
            var existing = await _unitOfWork.Repository<Domain.Entities.Department>()
                .GetSingleProjectedAsync(x => new { x.Name }, spec, cancellationToken);

            if (existing is not null)
                return BaseApiResponse.Fail(400, "A department with this name already exists in the company.");

            var department = new Domain.Entities.Department
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                CompanyId = companyId,
                Status = Domain.Enum.EntityStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Domain.Entities.Department>().AddAsync(department, cancellationToken);
            await _unitOfWork.SaveChangeAsync(cancellationToken);

            return BaseApiResponse.Success(201, "Department created successfully.");
        }
    }
}
