using Application.Dtos.Response;
using Domain.Attendence;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Attendance;
using Domain.Specification.Params;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Attendance.Queries.GetSummary
{
    public class GetAttendanceSummaryHandler : IRequestHandler<GetAttendanceSummaryQuery, BaseApiResponse<AttendanceSummaryDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetAttendanceSummaryHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse<AttendanceSummaryDto>> Handle(GetAttendanceSummaryQuery request, CancellationToken cancellationToken)
        {
            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(request.EmployeeId);
            if (employee is null || employee.IsDeleted)
                return BaseApiResponse<AttendanceSummaryDto>.Fail(404, "Employee not found.");

            // Employee: only own summary
            if (request.CurrentUserRole == "Employee" && employee.UserId != request.CurrentUserId)
                return BaseApiResponse<AttendanceSummaryDto>.Fail(403, "You can only view your own summary.");

            var from = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = from.AddMonths(1).AddTicks(-1);

            var parms = new AttendancePaginationParams
            {
                EmployeeId = request.EmployeeId,
                From = from,
                To = to,
                PageIndex = 1,
                PageSize = int.MaxValue
            };

            var spec = new AttendancePaginationSpecification(parms);

            var records = await _uow.Repository<Domain.Entities.Attendance>()
                .GetProjectedAsync(a => new
                {
                    a.WorkedHours,
                    a.OvertimeHours,
                    a.CheckOut
                }, spec);

            var completedRecords = records.Where(r => r.CheckOut.HasValue).ToList();

            var data = new AttendanceSummaryDto
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.User.FirstName + " " + employee.User.LastName,
                TotalDays = completedRecords.Count,
                TotalWorkedHours = Math.Round(completedRecords.Sum(r => r.WorkedHours) ?? 0, 2),
                TotalOvertimeHours = Math.Round(completedRecords.Sum(r => r.OvertimeHours), 2),
                Month = request.Month,
                Year = request.Year
            };

            return new BaseApiResponse<AttendanceSummaryDto>
            {
                Data = data,
                Message = "Attendance summary retrieved successfully.",
                StatusCode = 200
            };
        }
    }
}
