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

            var companySpec = new CompanySpecification(position.CompanyId);
            var company = await _uow.Repository<Domain.Entities.Company>().GetSingleProjectedAsync(c => new
            {
                c.OwnerId
            }, companySpec);
            if (!request.IsAdmin && company!.OwnerId != request.OwnerId)
                return BaseApiResponse.Fail(403, "You are not allowed to update this position.");

            var title = request.Dto.Title.Trim();
            var spec = new PositionSpecification(title, position.CompanyId, position.Id);
            var exists = await _uow.Repository<Domain.Entities.Position>()
                .GetSingleProjectedAsync(x => new { x.Id }, spec);

            if (exists is not null)
                return BaseApiResponse.Fail(400, "A position with this title already exists in the company.");

            position.Title = title;
            position.Description = request.Dto.Description?.Trim();
            position.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(200, "Position updated successfully.");
        }
    }
}
