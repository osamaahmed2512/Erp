using Application.Dtos.Response;
using Application.UseCases.WorkingSchedule.Common;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.WorkingSchedule;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Commands.Update;

public class UpdateWorkingScheduleHandler : IRequestHandler<UpdateWorkingScheduleCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateWorkingScheduleHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<BaseApiResponse> Handle(UpdateWorkingScheduleCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var schedule = await _uow.Repository<Domain.Entities.WorkingSchedule>()
            .GetByIdSpecAsync(new WorkingScheduleSpecification(request.Id, true));
        if (schedule is null) return BaseApiResponse.Fail(404, "Working schedule not found.");
        if (!request.IsSuperAdmin && schedule.Company.OwnerId != request.UserId)
            return BaseApiResponse.Fail(403, "You are not allowed to update this working schedule.");

        var duplicateSpec = new WorkingScheduleSpecification(dto.Name.Trim(), schedule.CompanyId, schedule.Id);
        if (await _uow.Repository<Domain.Entities.WorkingSchedule>().AnyAsync(duplicateSpec))
            return BaseApiResponse.Fail(400, "A working schedule with this name already exists in the company.");

        schedule.Name = dto.Name.Trim();
        schedule.IsActive = dto.IsActive;
        schedule.EffectiveFrom = dto.EffectiveFrom.Date;
        schedule.EffectiveTo = dto.EffectiveTo?.Date;
        schedule.UpdatedAt = DateTime.UtcNow;

        var incomingDays = dto.Days.ToDictionary(d => d.DayOfWeek);
        foreach (var day in schedule.Days)
        {
            var value = incomingDays[day.DayOfWeek];
            day.IsWorkingDay = value.IsWorkingDay;
            day.StartTime = value.IsWorkingDay ? value.StartTime : null;
            day.EndTime = value.IsWorkingDay ? value.EndTime : null;
            day.BreakMinutes = value.IsWorkingDay ? value.BreakMinutes : 0;
            day.UpdatedAt = DateTime.UtcNow;
        }

        var existingDays = schedule.Days.Select(d => d.DayOfWeek).ToHashSet();
        var missingDays = dto.Days.Where(d => !existingDays.Contains(d.DayOfWeek));
        foreach (var day in WorkingScheduleDayMapper.Map(missingDays))
            schedule.Days.Add(day);

        await _uow.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(200, "Working schedule updated successfully.");
    }
}
