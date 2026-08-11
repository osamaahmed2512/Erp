using Application.Dtos.Pagination;
using Domain.Attendence;
using Domain.Specification.Params;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Attendance.Queries.GetAll
{
    public class GetAllAttendanceQuery : IRequest<PaginationDTO<AttendanceDto>>
    {
        public AttendancePaginationParams PaginationParams { get; set; }
        public GetAllAttendanceQuery(AttendancePaginationParams p) => PaginationParams = p;
    }
}
