using Application.Dtos.Company;
using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Queries.GetById
{
    public class GetCompanyByIdQuery: IRequest<BaseApiResponse<CompanyDetailsDto>>
    {
        public Guid Id { get; set; }
    }
}
