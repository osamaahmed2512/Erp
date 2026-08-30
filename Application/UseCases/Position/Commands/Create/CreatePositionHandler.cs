using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Position;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Commands.Create
{
    public class CreatePositionHandler : IRequestHandler<CreatePositionCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        public CreatePositionHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var title = dto.Title.Trim();
            var department = await _uow.Repository<Domain.Entities.Department>().GetByIdAsync(dto.DepartmentId);
            if (department is null || department.Status == Domain.Enum.EntityStatus.Deleted)
                return BaseApiResponse.Fail(404, "Department not found.");
            if (department.CompanyId != request.CompanyId)
                return BaseApiResponse.Fail(400, "Department does not belong to the active company.");

            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(department.CompanyId);
            if (company is null)
                return BaseApiResponse.Fail(404, "Department company not found.");

            if (!request.HasGlobalAccess && company.OwnerId != request.OwnerId)
                return BaseApiResponse.Fail(403, "You are not allowed to add positions to this department.");

            var spec = new PositionSpecification(title, department.Id);
            var exists = await _uow.Repository<Domain.Entities.Position>()
                .GetSingleProjectedAsync(x => new { x.Id }, spec);

            if (exists is not null)
                return BaseApiResponse.Fail(400, "A position with this title already exists in the department.");

            var position = new Domain.Entities.Position
            {
                Title = title,
                Description = dto.Description?.Trim(),
                DepartmentId = department.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Repository<Domain.Entities.Position>().AddAsync(position);
            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(201, "Position created successfully.");
        }
    }
}
