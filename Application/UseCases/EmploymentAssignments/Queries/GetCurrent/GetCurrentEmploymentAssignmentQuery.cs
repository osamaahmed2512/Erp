using Application.Dtos.EmploymentAssignment;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.EmploymentAssignments.Queries.GetCurrent
{
    public class GetCurrentEmploymentAssignmentQuery : IRequest<BaseApiResponse<EmploymentAssignmentDto>>
    {
        public Guid EmployeeId { get; set; }
        public DateOnly EffectiveDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public Guid CurrentUserId { get; set; }
        public bool HasGlobalAccess { get; set; }
    }
}
