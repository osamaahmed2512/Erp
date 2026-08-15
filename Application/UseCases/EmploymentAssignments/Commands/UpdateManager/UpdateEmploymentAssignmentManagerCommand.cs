using Application.Dtos.EmploymentAssignment;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.EmploymentAssignments.Commands.UpdateManager
{
    public class UpdateEmploymentAssignmentManagerCommand : IRequest<BaseApiResponse<EmploymentAssignmentDto>>
    {
        public Guid EmployeeId { get; set; }
        public Guid AssignmentId { get; set; }
        public Guid? ManagerId { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool HasGlobalAccess { get; set; }
    }
}
