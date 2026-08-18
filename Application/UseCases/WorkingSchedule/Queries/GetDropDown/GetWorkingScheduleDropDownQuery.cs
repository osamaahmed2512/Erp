using Application.Dtos.DropDown;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Queries.GetDropDown;

public record GetWorkingScheduleDropDownQuery(
    Guid? CompanyId,
    Guid UserId,
    bool IsSuperAdmin) : IRequest<List<DropDownDto>>;
