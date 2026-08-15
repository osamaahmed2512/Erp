using Application.Dtos.EmploymentAssignment;
using Application.Dtos.Response;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Employee;
using Domain.Specification.EmploymentAssignment;
using MediatR;

namespace Application.UseCases.EmploymentAssignments.Commands.Create
{
    public class CreateEmploymentAssignmentHandler
        : IRequestHandler<CreateEmploymentAssignmentCommand, BaseApiResponse<EmploymentAssignmentDto>>
    {
        private readonly IUnitOfWork _uow;

        public CreateEmploymentAssignmentHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<BaseApiResponse<EmploymentAssignmentDto>> Handle(
            CreateEmploymentAssignmentCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Dto.EffectiveFrom == default)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(400, "Effective date is required.");

            var employee = await _uow.Repository<Domain.Entities.Employee>()
                .GetByIdSpecAsync(new EmployeeSpecification(request.EmployeeId));
            if (employee is null || employee.IsDeleted)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Employee not found.");

            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(employee.CompanyId);
            if (company is null)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Employee company not found.");

            if (!request.HasGlobalAccess && company.OwnerId != request.CurrentUserId)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(403, "You are not allowed to manage this employee.");

            var department = await _uow.Repository<Domain.Entities.Department>().GetByIdAsync(request.Dto.DepartmentId);
            if (department is null || department.Status == EntityStatus.Deleted)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Department not found.");

            if (department.CompanyId != employee.CompanyId)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(400, "Department must belong to the employee's company.");

            var position = await _uow.Repository<Domain.Entities.Position>().GetByIdAsync(request.Dto.PositionId);
            if (position is null || position.Status == EntityStatus.Deleted)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Position not found.");

            if (position.Status != EntityStatus.Active)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(400, "Position must be active.");

            if (position.DepartmentId != department.Id)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(400, "Position must belong to the selected department.");

            Domain.Entities.Employee? manager = null;
            if (request.Dto.ManagerId.HasValue)
            {
                if (request.Dto.ManagerId.Value == employee.Id)
                    return BaseApiResponse<EmploymentAssignmentDto>.Fail(400, "Employee cannot be their own manager.");

                manager = await _uow.Repository<Domain.Entities.Employee>()
                    .GetByIdSpecAsync(new EmployeeSpecification(request.Dto.ManagerId.Value));
                if (manager is null || manager.IsDeleted)
                    return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Manager not found.");

                if (manager.Status != EmployeeStatus.Active)
                    return BaseApiResponse<EmploymentAssignmentDto>.Fail(400, "Manager must be active.");

                if (manager.CompanyId != employee.CompanyId)
                    return BaseApiResponse<EmploymentAssignmentDto>.Fail(400, "Manager must belong to the employee's company.");
            }

            var assignmentSpec = new EmploymentAssignmentSpecification(employee.Id);
            var assignments = (await _uow.Repository<Domain.Entities.EmploymentAssignment>()
                .GetAllWithSpecAsync(assignmentSpec, asNoTracking: false)).ToList();

            if (assignments.Any(a => a.EffectiveFrom == request.Dto.EffectiveFrom))
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(409, "An assignment already starts on this effective date.");

            var previousAssignment = assignments
                .Where(a => a.EffectiveFrom < request.Dto.EffectiveFrom)
                .OrderByDescending(a => a.EffectiveFrom)
                .FirstOrDefault();

            var nextAssignment = assignments
                .Where(a => a.EffectiveFrom > request.Dto.EffectiveFrom)
                .OrderBy(a => a.EffectiveFrom)
                .FirstOrDefault();

            if (previousAssignment is not null &&
                (!previousAssignment.EffectiveTo.HasValue || previousAssignment.EffectiveTo >= request.Dto.EffectiveFrom))
            {
                previousAssignment.EffectiveTo = request.Dto.EffectiveFrom.AddDays(-1);
                previousAssignment.UpdatedAt = DateTime.UtcNow;
            }

            var assignment = new Domain.Entities.EmploymentAssignment
            {
                EmployeeId = employee.Id,
                DepartmentId = department.Id,
                PositionId = position.Id,
                ManagerId = manager?.Id,
                EffectiveFrom = request.Dto.EffectiveFrom,
                EffectiveTo = nextAssignment?.EffectiveFrom.AddDays(-1),
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Repository<Domain.Entities.EmploymentAssignment>().AddAsync(assignment);
            await _uow.SaveChangeAsync(cancellationToken);

            return new BaseApiResponse<EmploymentAssignmentDto>
            {
                StatusCode = 201,
                Message = "Employment assignment created successfully.",
                Data = Map(assignment, employee, department, position, manager)
            };
        }

        private static EmploymentAssignmentDto Map(
            Domain.Entities.EmploymentAssignment assignment,
            Domain.Entities.Employee employee,
            Domain.Entities.Department department,
            Domain.Entities.Position position,
            Domain.Entities.Employee? manager)
        {
            return new EmploymentAssignmentDto
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
            };
        }
    }
}
