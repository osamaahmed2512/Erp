using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Attendance;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Attendance.Commands.CheckIn
{
    public class CheckInHandler : IRequestHandler<CheckInCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        public CheckInHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse> Handle(CheckInCommand request, CancellationToken cancellationToken)
        {
            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(request.Dto.EmployeeId);
            if (employee is null || employee.IsDeleted)
                return BaseApiResponse.Fail(404, "Employee not found.");

            // Employee role: can only check in themselves
            if (request.CurrentUserRole == "Employee" && employee.UserId != request.CurrentUserId)
                return BaseApiResponse.Fail(403, "You can only check in yourself.");

            // Check for open attendance (already checked in, not yet checked out)
            var openSpec = new AttendanceSpecification(employee.Id, openOnly: true);
            var openAttendance = await _uow.Repository<Domain.Entities.Attendance>()
                .GetSingleProjectedAsync(x => new { x.Id }, openSpec);

            if (openAttendance is not null)
                return BaseApiResponse.Fail(400, "Employee already has an open check-in. Please check out first.");

            var attendance = new Domain.Entities.Attendance
            {
                EmployeeId = employee.Id,
                CheckIn = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Repository<Domain.Entities.Attendance>().AddAsync(attendance);
            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(201, "Checked in successfully.");
        }
    }
}
