using Application.Dtos.EmploymentAssignment;
using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.EmploymentAssignment;
using MediatR;

namespace Application.UseCases.EmploymentAssignments.Queries.GetCurrent
{
    public class GetCurrentEmploymentAssignmentHandler
        : IRequestHandler<GetCurrentEmploymentAssignmentQuery, BaseApiResponse<EmploymentAssignmentDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetCurrentEmploymentAssignmentHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<BaseApiResponse<EmploymentAssignmentDto>> Handle(
            GetCurrentEmploymentAssignmentQuery request,
            CancellationToken cancellationToken)
        {
            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(request.EmployeeId);
            if (employee is null || employee.IsDeleted)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Employee not found.");

            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(employee.CompanyId);
            if (company is null)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "Employee company not found.");

            if (!request.HasGlobalAccess && company.OwnerId != request.CurrentUserId)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(403, "You are not allowed to view this employee.");

            var effectiveDate = request.EffectiveDate == default
                ? DateOnly.FromDateTime(DateTime.UtcNow)
                : request.EffectiveDate;

            var spec = new EmploymentAssignmentSpecification(employee.Id, effectiveDate);
            var data = await _uow.Repository<Domain.Entities.EmploymentAssignment>()
                .GetSingleProjectedAsync(a => new EmploymentAssignmentDto
                {
                    Id = a.Id,
                    EmployeeId = a.EmployeeId,
                    EmployeeName = (a.Employee.User.FirstName + " " + a.Employee.User.LastName).Trim(),
                    DepartmentId = a.DepartmentId,
                    DepartmentName = a.Department.Name,
                    PositionId = a.PositionId,
                    PositionName = a.Position.Title,
                    ManagerId = a.ManagerId,
                    ManagerName = a.Manager == null
                        ? null
                        : (a.Manager.User.FirstName + " " + a.Manager.User.LastName).Trim(),
                    EffectiveFrom = a.EffectiveFrom,
                    EffectiveTo = a.EffectiveTo
                }, spec);

            if (data is null)
                return BaseApiResponse<EmploymentAssignmentDto>.Fail(404, "No employment assignment was found for this date.");

            return new BaseApiResponse<EmploymentAssignmentDto>
            {
                StatusCode = 200,
                Message = "Current employment assignment retrieved successfully.",
                Data = data
            };
        }
    }
}
