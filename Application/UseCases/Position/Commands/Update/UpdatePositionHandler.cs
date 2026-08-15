using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using Domain.Specification.Position;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Commands.Update
{
    public class UpdatePositionHandler : IRequestHandler<UpdatePositionCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        public UpdatePositionHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
        {
            var position = await _uow.Repository<Domain.Entities.Position>().GetByIdAsync(request.PositionId);
            if (position is null || position.Status == Domain.Enum.EntityStatus.Deleted)
                return BaseApiResponse.Fail(404, "Position not found.");

            var department = await _uow.Repository<Domain.Entities.Department>().GetByIdAsync(request.Dto.DepartmentId);
            if (department is null || department.Status == Domain.Enum.EntityStatus.Deleted)
                return BaseApiResponse.Fail(404, "Department not found.");

            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(department.CompanyId);
            if (company is null)
                return BaseApiResponse.Fail(404, "Department company not found.");

            if (!request.IsAdmin && company.OwnerId != request.OwnerId)
                return BaseApiResponse.Fail(403, "You are not allowed to update this position.");

            var title = request.Dto.Title.Trim();
            var spec = new PositionSpecification(title, department.Id, position.Id);
            var exists = await _uow.Repository<Domain.Entities.Position>()
                .GetSingleProjectedAsync(x => new { x.Id }, spec);

            if (exists is not null)
                return BaseApiResponse.Fail(400, "A position with this title already exists in the department.");

            position.Title = title;
            position.Description = request.Dto.Description?.Trim();
            position.DepartmentId = department.Id;
            position.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(200, "Position updated successfully.");
        }
    }
}
