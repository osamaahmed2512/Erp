using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.WorkingSchedule;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Commands.Delete;

public class DeleteWorkingScheduleHandler : IRequestHandler<DeleteWorkingScheduleCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _uow;

    public DeleteWorkingScheduleHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<BaseApiResponse> Handle(DeleteWorkingScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _uow.Repository<Domain.Entities.WorkingSchedule>()
            .GetByIdSpecAsync(new WorkingScheduleSpecification(request.Id, true));
        if (schedule is null) return BaseApiResponse.Fail(404, "Working schedule not found.");
        if (!request.IsSuperAdmin && schedule.Company.OwnerId != request.UserId)
            return BaseApiResponse.Fail(403, "You are not allowed to delete this working schedule.");

        var isAssigned = await _uow.Repository<Domain.Entities.EmploymentAssignment>()
            .AnyAsync(a => a.WorkingScheduleId == request.Id);
        if (isAssigned)
            return BaseApiResponse.Fail(409, "This schedule is assigned to employees and cannot be deleted.");

        await _uow.Repository<Domain.Entities.WorkingSchedule>().DeleteAsync(schedule);
        await _uow.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(200, "Working schedule deleted successfully.");
    }
}
