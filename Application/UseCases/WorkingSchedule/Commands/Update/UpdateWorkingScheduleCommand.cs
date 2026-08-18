using Application.Dtos.Response;
using Application.Dtos.WorkingSchedule;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Commands.Update;

public record UpdateWorkingScheduleCommand(
    Guid Id,
    UpdateWorkingScheduleDto Dto,
    Guid UserId,
    bool IsSuperAdmin) : IRequest<BaseApiResponse>;
