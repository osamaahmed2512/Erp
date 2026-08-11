using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using MediatR;


namespace Application.UseCases.Position.Commands.Status
{
    public class PositionUpdateStatusHandler : IRequestHandler<PositionUpdateStatusCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        public PositionUpdateStatusHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse> Handle(PositionUpdateStatusCommand request, CancellationToken cancellationToken)
        {
            var position = await _uow.Repository<Domain.Entities.Position>().GetByIdAsync(request.Id);
            if (position is null || position.Status == Domain.Enum.EntityStatus.Deleted)
                return BaseApiResponse.Fail(404, "Position not found.");

            var companySpec = new CompanySpecification(position.CompanyId);
            var company = await _uow.Repository<Domain.Entities.Company>().GetSingleProjectedAsync(c => new
            {
                c.OwnerId
            }, companySpec);
            if (!request.IsAdmin && company!.OwnerId != request.OwnerId)
                return BaseApiResponse.Fail(403, "You are not allowed to change the status of this position.");

            position.Status = request.Status.ToLower() == "active"
                ? Domain.Enum.EntityStatus.Active
                : Domain.Enum.EntityStatus.Inactive;

            position.UpdatedAt = DateTime.UtcNow;
            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(200, $"Position status changed to {position.Status} successfully.");
        }
    }
}
