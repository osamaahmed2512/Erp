using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Attendance;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Attendance.Commands.CheckOut
{
    public class CheckOutHandler : IRequestHandler<CheckOutCommand, BaseApiResponse>
    {
        // Standard working hours per day — overtime is calculated beyond this
        private const double StandardWorkHours = 8.0;
        private readonly IUnitOfWork _uow;
        public CheckOutHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse> Handle(CheckOutCommand request, CancellationToken cancellationToken)
        {
            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(request.Dto.EmployeeId);
            if (employee is null || employee.IsDeleted)
                return BaseApiResponse.Fail(404, "Employee not found.");

            // Employee role: can only check out themselves
            if (request.CurrentUserRole == "Employee" && employee.UserId != request.CurrentUserId)
                return BaseApiResponse.Fail(403, "You can only check out yourself.");

            // Find open attendance record
            var openSpec = new AttendanceSpecification(employee.Id, openOnly: true);
            var attendance = await _uow.Repository<Domain.Entities.Attendance>()
                .GetByIdSpecAsync(openSpec);

            if (attendance is null)
                return BaseApiResponse.Fail(400, "No open check-in found for this employee.");

            var checkOut = DateTime.UtcNow;
            var workedHours = (checkOut - attendance.CheckIn).TotalHours;

            attendance.CheckOut = checkOut;
            attendance.WorkedHours = Math.Round(workedHours, 2);
            attendance.OvertimeHours = workedHours > StandardWorkHours
                ? Math.Round(workedHours - StandardWorkHours, 2)
                : 0;
            attendance.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(200, $"Checked out successfully. Worked: {attendance.WorkedHours}h, Overtime: {attendance.OvertimeHours}h.");
        }
    }
}
