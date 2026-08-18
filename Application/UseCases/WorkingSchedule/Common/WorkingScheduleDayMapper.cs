using Application.Dtos.WorkingSchedule;

namespace Application.UseCases.WorkingSchedule.Common;

internal static class WorkingScheduleDayMapper
{
    public static List<Domain.Entities.WorkingScheduleDay> Map(IEnumerable<WorkingScheduleDayDto> days) =>
        days.Select(d => new Domain.Entities.WorkingScheduleDay
        {
            DayOfWeek = d.DayOfWeek,
            IsWorkingDay = d.IsWorkingDay,
            StartTime = d.IsWorkingDay ? d.StartTime : null,
            EndTime = d.IsWorkingDay ? d.EndTime : null,
            BreakMinutes = d.IsWorkingDay ? d.BreakMinutes : 0
        }).ToList();
}
