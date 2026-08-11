using Application.Dtos.DropDown;
using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using Domain.Specification.Params;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Queries.GetDropDown
{
    public class GetCompanyDropDownCommandHandler : IRequestHandler<GetCompanyDropDownCommand, List<DropDownDto>>

    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCompanyDropDownCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<DropDownDto>> Handle(GetCompanyDropDownCommand request, CancellationToken cancellationToken)
        {
            var spec = new CompanySpecification(request.OwnerId, true);

            var companies = await _unitOfWork.Repository<Domain.Entities.Company>()
                .GetProjectedAsync( c => new DropDownDto
                {
                    Id = c.Id,
                    Name = c.Name
                },spec);

            return companies;

        }
    }
}
