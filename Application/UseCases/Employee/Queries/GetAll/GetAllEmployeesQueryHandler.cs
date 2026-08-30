

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
        : IRequestHandler<GetAllEmployeesQuery, PaginationDTO<EmployeeResponse>>
    {
        private readonly IUnitOfWork _uow;

        public getallemployeesqueryhandler(IUnitOfWork uow) => _uow = uow;

        public async Task<PaginationDTO<EmployeeResponse>> Handle(
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
                                           CompanyId = e.CompanyId,
                                           ComapnyName = e.Company.Name,
                                           Email=e.User.Email,
                                           PhoneNumber=e.User.PhoneNumber,
                                           NationalityNumber=e.NationalityNumber,
                                           Status=e.Status.ToString()
                                       }, spec, cancellationToken);

            var totalcount = await _uow.Repository<Domain.Entities.Employee>()
                                       .CountWithSpec(countspec, cancellationToken);


            return new PaginationDTO<EmployeeResponse>
            {
                data = employees,
                TotalCount = totalcount,
                PageIndex = request.Params.PageIndex,
                PageSize = request.Params.PageSize
            };

        }
    }
}
