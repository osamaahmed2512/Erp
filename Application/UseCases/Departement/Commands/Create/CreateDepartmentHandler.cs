using Application.Dtos.Response;
using Application.Interfaces.InternalServices;
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
        private readonly ICurrentUserService _currentUserService;
        public CreateDepartmentHandler(IUnitOfWork unitOfWork,ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<BaseApiResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // Validate company exists and belongs to owner
            var company = await _unitOfWork.Repository<Domain.Entities.Company>().GetByIdAsync(dto.CompanyId);
            if (company is null)
                return BaseApiResponse.Fail(404, "Company not found.");

            if (company.OwnerId != request.OwnerId &&!_currentUserService.IsSuperAdmin)
                return BaseApiResponse.Fail(403, "You are not allowed to add departments to this company.");

            var spec = new DepartmentSpecification(dto.Name.Trim(), dto.CompanyId);
            var existing = await _unitOfWork.Repository<Domain.Entities.Department>()
                .GetSingleProjectedAsync(x => new { x.Name }, spec);

            if (existing is not null)
                return BaseApiResponse.Fail(400, "A department with this name already exists in the company.");

            var department = new Domain.Entities.Department
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                CompanyId = dto.CompanyId,
                Status = Domain.Enum.EntityStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Domain.Entities.Department>().AddAsync(department);
            await _unitOfWork.SaveChangeAsync(cancellationToken);

            return BaseApiResponse.Success(201, "Department created successfully.");
        }
    }
}
