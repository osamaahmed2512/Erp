using Application.Dtos.DropDown;
using Application.Dtos.Nationality;
using Application.Dtos.Pagination;
using Application.Dtos.Response;
using Domain.Entities;
using Domain.Interfaces.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Nationality.Queries.GetAll
{
    public class GetAllNationalitiesQueryHandler : IRequestHandler<GetAllNationalitiesQuery, List<DropDownDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllNationalitiesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<DropDownDto>> Handle(GetAllNationalitiesQuery request, CancellationToken cancellationToken)
        {
            var nationalities = _unitOfWork.Repository<Domain.Entities.Nationality>().GetQueryableWithSpec(null).Where(x =>x.IsActive)
                .Select(x => new DropDownDto
                {
                    Id = x.Id,
                    Name=x.NameAr
                }).ToList();
            return nationalities;
        }
    }
}
