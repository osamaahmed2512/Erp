using Application.Dtos.Pagination;
using Domain.Attendence;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Attendance;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Attendance.Queries.GetAll
{
    public class GetAllAttendanceHandler : IRequestHandler<GetAllAttendanceQuery, PaginationDTO<AttendanceDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetAllAttendanceHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<PaginationDTO<AttendanceDto>> Handle(GetAllAttendanceQuery request, CancellationToken cancellationToken)
        {
            var spec = new AttendancePaginationSpecification(request.PaginationParams);
            var countSpec = new AttendanceCountSpecification(request.PaginationParams);

            var records = await _uow.Repository<Domain.Entities.Attendance>()
                .GetProjectedAsync(a => new AttendanceDto
                {
                    Id = a.Id,
                    EmployeeName = a.Employee.User.FirstName + " " + a.Employee.User.LastName,
                    CheckIn = a.CheckIn,
                    CheckOut = a.CheckOut,
                    WorkedHours = a.WorkedHours,
                    OvertimeHours = a.OvertimeHours
                }, spec);

            var totalCount = await _uow.Repository<Domain.Entities.Attendance>().CountWithSpec(countSpec);

            return new PaginationDTO<AttendanceDto>
            {
                data = records,
                TotalCount = totalCount,
                PageIndex = request.PaginationParams.PageIndex,
                PageSize = request.PaginationParams.PageSize
            };
        }
    }
}
