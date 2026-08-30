using Application.Dtos.Position;
using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Commands.Update
{
    public class UpdatePositionCommand : IRequest<BaseApiResponse>
    {
        public Guid PositionId { get; set; }
        public UpdatePositionDto Dto { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsAdmin { get; set; }
        public Guid CompanyId { get; set; }
    }
}
