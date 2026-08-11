using Application.Dtos.Pagination;
using Application.Dtos.Position;
using Domain.Specification.Params;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Queries.GetAll
{
    public class GetAllPositionsQuery : IRequest<PaginationDTO<PositionDto>>
    {
        public PositionPaginationParams PaginationParams { get; set; }
        public GetAllPositionsQuery(PositionPaginationParams p) => PaginationParams = p;
    }
}
