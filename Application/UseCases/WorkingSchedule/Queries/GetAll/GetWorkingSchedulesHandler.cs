using Application.Dtos.Pagination;
using Application.Dtos.WorkingSchedule;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.WorkingSchedule;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Queries.GetAll;

public class GetWorkingSchedulesHandler : IRequestHandler<GetWorkingSchedulesQuery, PaginationDTO<WorkingScheduleListDto>>
{
    private readonly IUnitOfWork _uow;

    public GetWorkingSchedulesHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<PaginationDTO<WorkingScheduleListDto>> Handle(
        GetWorkingSchedulesQuery request,
        CancellationToken cancellationToken)
    {
        var countSpec = new WorkingScheduleSpecification(request.Parameters, request.UserId, request.IsSuperAdmin);
        var pageSpec = new WorkingSchedulePaginationSpecification(request.Parameters, request.UserId, request.IsSuperAdmin);

        var schedules = await _uow.Repository<Domain.Entities.WorkingSchedule>()
            .GetProjectedAsync(s => new WorkingScheduleListDto
            {
                Id = s.Id,
                Name = s.Name,
                CompanyId = s.CompanyId,
                CompanyName = s.Company.Name,
                IsActive = s.IsActive,
                EffectiveFrom = s.EffectiveFrom,
                EffectiveTo = s.EffectiveTo,
                WorkingDays = s.Days.Count(d => d.IsWorkingDay)
            }, pageSpec);

        return new PaginationDTO<WorkingScheduleListDto>
        {
            data = schedules,
            TotalCount = await _uow.Repository<Domain.Entities.WorkingSchedule>().CountWithSpec(countSpec),
            PageIndex = request.Parameters.PageIndex,
            PageSize = request.Parameters.PageSize
        };
    }
}
