using Application.Dtos.Pagination;
using Application.Dtos.Position;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Position;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Queries.GetAll
{
    public class GetAllPositionsHandler : IRequestHandler<GetAllPositionsQuery, PaginationDTO<PositionDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetAllPositionsHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<PaginationDTO<PositionDto>> Handle(GetAllPositionsQuery request, CancellationToken cancellationToken)
        {
            var spec = new PositionPaginationSpecification(request.PaginationParams);
            var countSpec = new PositionCountSpecification(request.PaginationParams);

            var positions = await _uow.Repository<Domain.Entities.Position>()
                .GetProjectedAsync(p => new PositionDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    CompanyName = p.Company.Name,
                    Status = p.Status.ToString(),
                    TotalEmployees = 0
                }, spec);

            var totalCount = await _uow.Repository<Domain.Entities.Position>().CountWithSpec(countSpec);

            return new PaginationDTO<PositionDto>
            {
                data = positions,
                TotalCount = totalCount,
                PageIndex = request.PaginationParams.PageIndex,
                PageSize = request.PaginationParams.PageSize
            };
        }
    }
}
