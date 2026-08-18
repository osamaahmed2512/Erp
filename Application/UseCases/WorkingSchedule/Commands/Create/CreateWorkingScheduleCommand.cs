using Application.Dtos.Response;
using Application.Dtos.WorkingSchedule;
using MediatR;

namespace Application.UseCases.WorkingSchedule.Commands.Create;

public record CreateWorkingScheduleCommand(
    CreateWorkingScheduleDto Dto,
    Guid UserId,
    bool IsSuperAdmin) : IRequest<BaseApiResponse>;
