using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Commands.Delete;

public record DeleteWorkingScheduleCommand(
    Guid Id,
    Guid UserId,
    bool IsSuperAdmin) : IRequest<BaseApiResponse>;
