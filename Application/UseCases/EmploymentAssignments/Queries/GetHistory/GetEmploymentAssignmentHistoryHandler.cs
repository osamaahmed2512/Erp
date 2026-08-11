using Application.Dtos.EmploymentAssignment;
using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.EmploymentAssignment;
using MediatR;

namespace Application.UseCases.EmploymentAssignments.Queries.GetHistory
{
    public class GetEmploymentAssignmentHistoryHandler
        : IRequestHandler<GetEmploymentAssignmentHistoryQuery, BaseApiResponse<List<EmploymentAssignmentDto>>>
    {
        private readonly IUnitOfWork _uow;

        public GetEmploymentAssignmentHistoryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<BaseApiResponse<List<EmploymentAssignmentDto>>> Handle(
            GetEmploymentAssignmentHistoryQuery request,
            CancellationToken cancellationToken)
        {
            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(request.EmployeeId);
            if (employee is null || employee.IsDeleted)
                return BaseApiResponse<List<EmploymentAssignmentDto>>.Fail(404, "Employee not found.");

            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(employee.CompanyId);
            if (company is null)
                return BaseApiResponse<List<EmploymentAssignmentDto>>.Fail(404, "Employee company not found.");

            if (!request.HasGlobalAccess && company.OwnerId != request.CurrentUserId)
                return BaseApiResponse<List<EmploymentAssignmentDto>>.Fail(403, "You are not allowed to view this employee.");

            var spec = new EmploymentAssignmentSpecification(employee.Id);
            var data = await _uow.Repository<Domain.Entities.EmploymentAssignment>()
                .GetProjectedAsync(a => new EmploymentAssignmentDto
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

            return new BaseApiResponse<List<EmploymentAssignmentDto>>
            {
                StatusCode = 200,
                Message = "Employment assignment history retrieved successfully.",
                Data = data
            };
        }
    }
}
