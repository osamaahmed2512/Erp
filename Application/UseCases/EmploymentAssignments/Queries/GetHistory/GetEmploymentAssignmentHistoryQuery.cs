using Application.Dtos.EmploymentAssignment;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.EmploymentAssignments.Queries.GetHistory
{
    public class GetEmploymentAssignmentHistoryQuery : IRequest<BaseApiResponse<List<EmploymentAssignmentDto>>>
    {
        public Guid EmployeeId { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool HasGlobalAccess { get; set; }
    }
}
