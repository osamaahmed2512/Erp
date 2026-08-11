using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Commands.Status
{
    public class PositionUpdateStatusCommand : IRequest<BaseApiResponse>
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
