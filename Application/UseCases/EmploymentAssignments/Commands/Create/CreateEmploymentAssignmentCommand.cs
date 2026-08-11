using Application.Dtos.EmploymentAssignment;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.EmploymentAssignments.Commands.Create
{
    public class CreateEmploymentAssignmentCommand : IRequest<BaseApiResponse<EmploymentAssignmentDto>>
    {
        public Guid EmployeeId { get; set; }
        public CreateEmploymentAssignmentDto Dto { get; set; } = null!;
        public Guid CurrentUserId { get; set; }
        public bool HasGlobalAccess { get; set; }
    }
}
