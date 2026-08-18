using Application.Dtos.DropDown;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Params;
using Domain.Specification.WorkingSchedule;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Queries.GetDropDown;

public class GetWorkingScheduleDropDownHandler
    : IRequestHandler<GetWorkingScheduleDropDownQuery, List<DropDownDto>>
{
    private readonly IUnitOfWork _uow;

    public GetWorkingScheduleDropDownHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public Task<List<DropDownDto>> Handle(
        GetWorkingScheduleDropDownQuery request,
        CancellationToken cancellationToken)
    {
        var parameters = new WorkingSchedulePaginationParams
        {
            CompanyId = request.CompanyId,
            PageSize = int.MaxValue
        };
        var spec = new WorkingScheduleSpecification(parameters, request.UserId, request.IsSuperAdmin);

        return _uow.Repository<Domain.Entities.WorkingSchedule>()
            .GetProjectedAsync(s => new DropDownDto { Id = s.Id, Name = s.Name }, spec);
    }
}
