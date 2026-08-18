using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.WorkingSchedule;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Commands.Status;

public class ChangeWorkingScheduleStatusHandler : IRequestHandler<ChangeWorkingScheduleStatusCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _uow;

    public ChangeWorkingScheduleStatusHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<BaseApiResponse> Handle(ChangeWorkingScheduleStatusCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _uow.Repository<Domain.Entities.WorkingSchedule>()
            .GetByIdSpecAsync(new WorkingScheduleSpecification(request.Id, false));
        if (schedule is null) return BaseApiResponse.Fail(404, "Working schedule not found.");
        if (!request.IsSuperAdmin && schedule.Company.OwnerId != request.UserId)
            return BaseApiResponse.Fail(403, "You are not allowed to update this working schedule.");

        schedule.IsActive = request.IsActive;
        schedule.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(200, "Working schedule status updated successfully.");
    }
}
