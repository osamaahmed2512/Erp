using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Commands.Status;

public record ChangeWorkingScheduleStatusCommand(
    Guid Id,
    bool IsActive,
    Guid UserId,
    bool IsSuperAdmin) : IRequest<BaseApiResponse>;
