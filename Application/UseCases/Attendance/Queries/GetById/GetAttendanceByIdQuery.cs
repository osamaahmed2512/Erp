using Application.Dtos.Response;
using Domain.Attendence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Attendance.Queries.GetById
{
    public class GetAttendanceByIdQuery : IRequest<BaseApiResponse<AttendanceDetailsDto>>
    {
        public Guid Id { get; set; }
        public Guid CurrentUserId { get; set; }
        public string CurrentUserRole { get; set; }
    }
}
