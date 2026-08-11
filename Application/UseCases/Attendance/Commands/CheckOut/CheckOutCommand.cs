using Application.Dtos.Response;
using Domain.Attendence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Attendance.Commands.CheckOut
{
    public class CheckOutCommand : IRequest<BaseApiResponse>
    {
        public CheckOutDto Dto { get; set; }
        public Guid CurrentUserId { get; set; }
        public string CurrentUserRole { get; set; }
    }
}
