using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Commands.Delete
{
    public class DeletePositionCommand : IRequest<BaseApiResponse>
    {
        public Guid PositionId { get; set; }
    }
}
