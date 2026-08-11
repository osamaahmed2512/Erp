using Application.Dtos.DropDown;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Params;
using Domain.Specification.Position;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Queries.GetDropDown
{
    public class GetPositionDropDownHandler : IRequestHandler<GetPositionDropDownQuery, List<DropDownDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetPositionDropDownHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<List<DropDownDto>> Handle(GetPositionDropDownQuery request, CancellationToken cancellationToken)
        {
            var parms = new PositionPaginationParams
            {
                CompanyId = request.CompanyId,
                PageIndex = 1,
                PageSize = int.MaxValue
            };
            var spec = new PositionSpecification(parms);
            return await _uow.Repository<Domain.Entities.Position>()
                .GetProjectedAsync(p => new DropDownDto { Id = p.Id, Name = p.Title }, spec);
        }
    }
}
