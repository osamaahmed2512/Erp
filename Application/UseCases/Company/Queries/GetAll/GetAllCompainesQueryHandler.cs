using Application.Dtos.Company;
using Application.Dtos.Pagination;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using Domain.Specification.Params;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Queries.GetAll
{
    public class GetAllCompainesQueryHandler:IRequestHandler<GetAllCompaniesQuery, PaginationDTO<CompanyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllCompainesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }

        public async Task<PaginationDTO<CompanyDto>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            var spec = new CompanyPaginationSpecification(request.paginationParams);
            var countSpec =new CompanyCountSpecification(request.paginationParams);

            var companies = await _unitOfWork.Repository<Domain.Entities.Company>()
                .GetProjectedAsync(c => new CompanyDto
                {   Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone =c.Phone,
                    OwnerName= c.Owner.FirstName+" "+c.Owner.LastName,
                    Status = c.Status.ToString(),
                    Website=c.Website,
                    TotalEmployees = c.Employees.Count()
                },spec);
            var totalCount = await _unitOfWork.Repository<Domain.Entities.Company>()
                .CountWithSpec(countSpec);

            return new PaginationDTO<CompanyDto>
            {
                data = companies,
                TotalCount = totalCount,
                PageIndex = request.paginationParams.PageIndex,
                PageSize = request.paginationParams.PageSize
            };


        }
    }
}
