using Application.Dtos.Response;
using Domain.Attendence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Attendance.Queries.GetSummary
{
    public class GetAttendanceSummaryQuery : IRequest<BaseApiResponse<AttendanceSummaryDto>>
    {
        public Guid EmployeeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid CurrentUserId { get; set; }
        public string CurrentUserRole { get; set; }
    }
}
