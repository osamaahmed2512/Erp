using Application.Dtos.EmploymentAssignment;
using Application.Dtos.Response;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Employee;
using MediatR;

namespace Application.UseCases.EmploymentAssignments.Commands.UpdateManager
{
    public class UpdateEmploymentAssignmentManagerHandler
        : IRequestHandler<UpdateEmploymentAssignmentManagerCommand, BaseApiResponse<EmploymentAssignmentDto>>
    {
        private readonly IUnitOfWork _uow;
        public UpdateEmploymentAssignmentManagerHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<BaseApiResponse<EmploymentAssignmentDto>> Handle(
            UpdateEmploymentAssignmentManagerCommand request,
            CancellationToken cancellationToken)
        {
            var employee = await _uow.Repository<Domain.Entities.Employee>()
                .GetByIdSpecAsync(new EmployeeSpecification(request.EmployeeId));
            if (employee is null || employee.IsDeleted)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Employee not found.");

            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(employee.CompanyId);
            if (company is null)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Employee company not found.");

            if (!request.HasGlobalAccess && company.OwnerId != request.CurrentUserId)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(403, "You are not allowed to manage this employee.");

            var assignment = await _uow.Repository<Domain.Entities.EmploymentAssignment>().GetByIdAsync(request.AssignmentId);
            if (assignment is null || assignment.EmployeeId != employee.Id)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Employment assignment not found.");

            Domain.Entities.Employee? manager = null;
            if (request.ManagerId.HasValue)
            {
                if (request.ManagerId.Value == employee.Id)
                    return BaseApiResponse<EmploymentAssignmentDto>.Fail(400, "Employee cannot be their own manager.");

                manager = await _uow.Repository<Domain.Entities.Employee>()
                    .GetByIdSpecAsync(new EmployeeSpecification(request.ManagerId.Value));
                if (manager is null || manager.IsDeleted)
                    return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Manager not found.");
                if (manager.Status != EmployeeStatus.Active)
                    return BaseApiResponse<EmploymentAssignmentDto>.Fail(400, "Manager must be active.");
                if (manager.CompanyId != employee.CompanyId)
                    return BaseApiResponse<EmploymentAssignmentDto>.Fail(400, "Manager must belong to the employee's company.");
            }

            var department = await _uow.Repository<Domain.Entities.Department>().GetByIdAsync(assignment.DepartmentId);
            var position = await _uow.Repository<Domain.Entities.Position>().GetByIdAsync(assignment.PositionId);
            if (department is null || position is null)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Assignment organization data was not found.");

            assignment.ManagerId = manager?.Id;
            assignment.UpdatedAt = DateTime.UtcNow;
            await _uow.SaveChangeAsync(cancellationToken);

            return new BaseApiResponse<EmploymentAssignmentDto>
            {
                StatusCode = 200,
                Message = "Manager updated successfully.",
                Data = new EmploymentAssignmentDto
                {
                    Id = assignment.Id,
                    EmployeeId = employee.Id,
                    EmployeeName = $"{employee.User.FirstName} {employee.User.LastName}".Trim(),
                    DepartmentId = department.Id,
                    DepartmentName = department.Name,
                    PositionId = position.Id,
                    PositionName = position.Title,
                    ManagerId = manager?.Id,
                    ManagerName = manager is null ? null : $"{manager.User.FirstName} {manager.User.LastName}".Trim(),
                    EffectiveFrom = assignment.EffectiveFrom,
                    EffectiveTo = assignment.EffectiveTo
                }
            };
        }
    }
}
