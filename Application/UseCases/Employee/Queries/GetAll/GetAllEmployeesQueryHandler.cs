

using Application.Dtos.Employee;
using Application.Dtos.Pagination;
using Application.Dtos.Response;
using Application.UseCases.Employee.Queries.GetAll;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Employee;
using MediatR;

namespace application.usecases.employee.queries.getall
{
    public class getallemployeesqueryhandler
        : IRequestHandler<GetAllEmployeesQuery, BaseApiResponse<PaginationDTO<EmployeeResponse>>>
    {
        private readonly IUnitOfWork _uow;

        public getallemployeesqueryhandler(IUnitOfWork uow) => _uow = uow;

        public async Task<BaseApiResponse<PaginationDTO<EmployeeResponse>>> Handle(
            GetAllEmployeesQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new EmployeeSpecification(request.Params);
            var countspec = new EmployeeCountSpecification(request.Params);

            var employees = await _uow.Repository<Domain.Entities.Employee>()
                                       .GetProjectedAsync(e => new EmployeeResponse
                                       {
                                           Id = e.Id,
                                           FirstName = e.User.FirstName,
                                           LastName = e.User.LastName,
                                           ComapnyName = e.Company.Name
                                       },spec);

            var totalcount = await _uow.Repository<Domain.Entities.Employee>()
                                       .CountWithSpec(countspec);


            var result = new PaginationDTO<EmployeeResponse>
            {
                data = employees,
                TotalCount = totalcount,
                PageIndex = request.Params.PageIndex,
                PageSize = request.Params.PageSize
            };

            return new BaseApiResponse<PaginationDTO<EmployeeResponse>>
            {
                StatusCode = 200,
                Message = "Employees retrieved successfully.",
                Data = result
            };
        }
    }
}
