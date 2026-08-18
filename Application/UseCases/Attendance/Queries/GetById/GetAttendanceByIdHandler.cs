using Application.Dtos.Response;
using Domain.Attendence;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Employee;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Attendance.Queries.GetById
{
    public class GetAttendanceByIdHandler : IRequestHandler<GetAttendanceByIdQuery, BaseApiResponse<AttendanceDetailsDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetAttendanceByIdHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse<AttendanceDetailsDto>> Handle(GetAttendanceByIdQuery request, CancellationToken cancellationToken)
        {
            var attendance = await _uow.Repository<Domain.Entities.Attendance>().GetByIdAsync(request.Id);
            if (attendance is null)
                return BaseApiResponse<AttendanceDetailsDto>.Fail(404, "Attendance record not found.");

            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(attendance.EmployeeId);

            // Employee: only own record
            if (request.CurrentUserRole == "Employee" && employee.UserId != request.CurrentUserId)
                return BaseApiResponse<AttendanceDetailsDto>.Fail(403, "You can only view your own attendance.");
            var employeeSpec = new EmployeeSpecification(request.CurrentUserId);


            var data = new AttendanceDetailsDto
            {
                Id = attendance.Id,
                EmployeeId = attendance.EmployeeId,
                EmployeeName = employee.User.FirstName + " " + employee.User.LastName,
                CheckIn = attendance.CheckIn,
                CheckOut = attendance.CheckOut,
                WorkedHours = attendance.WorkedHours ?? 0,
                OvertimeHours = attendance.OvertimeHours,
                CreatedAt = attendance.CreatedAt,
                UpdatedAt = attendance.UpdatedAt
            };

            return new BaseApiResponse<AttendanceDetailsDto>
            {
                Data = data,
                Message = "Attendance record retrieved successfully.",
                StatusCode = 200
            };
        }
    }

}
