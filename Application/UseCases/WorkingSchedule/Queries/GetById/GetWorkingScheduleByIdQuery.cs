using Application.Dtos.Response;
using Application.Dtos.WorkingSchedule;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Queries.GetById;

public record GetWorkingScheduleByIdQuery(
    Guid Id,
    Guid UserId,
    bool IsSuperAdmin) : IRequest<BaseApiResponse<WorkingScheduleDetailsDto>>;
