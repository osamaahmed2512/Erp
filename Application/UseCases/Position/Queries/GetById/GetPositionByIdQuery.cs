using Application.Dtos.Position;
using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Queries.GetById
{
    public class GetPositionByIdQuery : IRequest<BaseApiResponse<PositionDetailsDto>>
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
