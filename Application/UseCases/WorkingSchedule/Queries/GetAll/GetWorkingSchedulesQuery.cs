using Application.Dtos.Pagination;
using Application.Dtos.WorkingSchedule;
using Domain.Specification.Params;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Queries.GetAll;

public record GetWorkingSchedulesQuery(
    WorkingSchedulePaginationParams Parameters,
    Guid UserId,
    bool IsSuperAdmin) : IRequest<PaginationDTO<WorkingScheduleListDto>>;
