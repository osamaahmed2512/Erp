using Application.Dtos.Response;
using Application.Dtos.WorkingSchedule;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.WorkingSchedule;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Queries.GetById;

public class GetWorkingScheduleByIdHandler
    : IRequestHandler<GetWorkingScheduleByIdQuery, BaseApiResponse<WorkingScheduleDetailsDto>>
{
    private readonly IUnitOfWork _uow;

    public GetWorkingScheduleByIdHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<BaseApiResponse<WorkingScheduleDetailsDto>> Handle(
        GetWorkingScheduleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var schedule = await _uow.Repository<Domain.Entities.WorkingSchedule>()
            .GetByIdSpecAsync(new WorkingScheduleSpecification(request.Id, true));
        if (schedule is null)
            return BaseApiResponse<WorkingScheduleDetailsDto>.Fail(404, "Working schedule not found.");
        if (!request.IsSuperAdmin && schedule.Company.OwnerId != request.UserId)
            return BaseApiResponse<WorkingScheduleDetailsDto>.Fail(403, "You are not allowed to access this working schedule.");

        var data = new WorkingScheduleDetailsDto
        {
            Id = schedule.Id,
            Name = schedule.Name,
            CompanyId = schedule.CompanyId,
            CompanyName = schedule.Company.Name,
            IsActive = schedule.IsActive,
            EffectiveFrom = schedule.EffectiveFrom,
            EffectiveTo = schedule.EffectiveTo,
            WorkingDays = schedule.Days.Count(d => d.IsWorkingDay),
            CreatedAt = schedule.CreatedAt,
            UpdatedAt = schedule.UpdatedAt,
            Days = schedule.Days.OrderBy(d => d.DayOfWeek).Select(d => new WorkingScheduleDayDto
            {
                DayOfWeek = d.DayOfWeek,
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                IsWorkingDay = d.IsWorkingDay,
                BreakMinutes = d.BreakMinutes
            }).ToList()
        };

        return new BaseApiResponse<WorkingScheduleDetailsDto>(
            200,
            "Working schedule retrieved successfully.",
            data);
    }
}
