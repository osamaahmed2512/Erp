using Application.Dtos.Response;
using Application.UseCases.WorkingSchedule.Common;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.WorkingSchedule;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Commands.Create;

public class CreateWorkingScheduleHandler : IRequestHandler<CreateWorkingScheduleCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateWorkingScheduleHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<BaseApiResponse> Handle(CreateWorkingScheduleCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(dto.CompanyId);
        if (company is null) return BaseApiResponse.Fail(404, "Company not found.");
        if (!request.IsSuperAdmin && company.OwnerId != request.UserId)
            return BaseApiResponse.Fail(403, "You are not allowed to add schedules to this company.");

        var duplicateSpec = new WorkingScheduleSpecification(dto.Name.Trim(), dto.CompanyId);
        if (await _uow.Repository<Domain.Entities.WorkingSchedule>().AnyAsync(duplicateSpec))
            return BaseApiResponse.Fail(400, "A working schedule with this name already exists in the company.");

        var schedule = new Domain.Entities.WorkingSchedule
        {
            Name = dto.Name.Trim(),
            CompanyId = dto.CompanyId,
            IsActive = dto.IsActive,
            EffectiveFrom = dto.EffectiveFrom.Date,
            EffectiveTo = dto.EffectiveTo?.Date,
            Days = WorkingScheduleDayMapper.Map(dto.Days)
        };

        await _uow.Repository<Domain.Entities.WorkingSchedule>().AddAsync(schedule);
        await _uow.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(201, "Working schedule created successfully.");
    }
}
