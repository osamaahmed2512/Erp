using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Commands.Delete
{
    public class DeletePositionHandler : IRequestHandler<DeletePositionCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        public DeletePositionHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse> Handle(DeletePositionCommand request, CancellationToken cancellationToken)
        {
            var position = await _uow.Repository<Domain.Entities.Position>().GetByIdAsync(request.PositionId);
            if (position is null || position.Status == Domain.Enum.EntityStatus.Deleted)
                return BaseApiResponse.Fail(404, "Position not found.");

            position.Status = Domain.Enum.EntityStatus.Deleted;
            position.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(200, "Position deleted successfully.");
        }
    }
}
