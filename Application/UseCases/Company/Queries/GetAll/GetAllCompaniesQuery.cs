using Application.Dtos.Company;
using Application.Dtos.Pagination;
using Domain.Specification.Params;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Queries.GetAll
{
    public class GetAllCompaniesQuery:IRequest<PaginationDTO<CompanyDto>>
    {
        public CompanyPaginationParams paginationParams { get; set; }
        public GetAllCompaniesQuery(CompanyPaginationParams PaginationParams)
        {
            paginationParams = PaginationParams;
        }
    }
}
