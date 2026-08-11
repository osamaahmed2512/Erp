using Application.Dtos.Employee;
using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Employee;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Employee.Queries.GetById
{
    public class GetEmployeeByIdQueryHandler
        : IRequestHandler<GetEmployeeByIdQuery, BaseApiResponse<EmployeeDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetEmployeeByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<BaseApiResponse<EmployeeDto>> Handle(
            GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new EmployeeSpecification(request.Id);
            var employee = await _uow.Repository<Domain.Entities.Employee>()
                                     .GetSingleProjectedAsync(e => new
                                     EmployeeDto
                                     {
                                         Id = e.Id,
                                         FirstName = e.User.FirstName,
                                         LastName = e.User.LastName,
                                         Email = e.User.Email,
                                         Phone = e.User.PhoneNumber,
                                         Address=e.Address,
                                         BirthDate = e.BirthDate,
                                         Gender = e.Gender,
                                         MartielStatus=e.MartielStatus,
                                         nationalityId= e.NationalityId,
                                         NationalityNumber =e.NationalityNumber,
                                         Status=e.Status,
                                         companyId=e.CompanyId,
                                         
                                     }, spec);

            if (employee is null)
                return new BaseApiResponse<EmployeeDto>(404, "Employee not found.");

            return new BaseApiResponse<EmployeeDto>(200, "Employee retrieved successfully.", employee);
        }
    }
}
