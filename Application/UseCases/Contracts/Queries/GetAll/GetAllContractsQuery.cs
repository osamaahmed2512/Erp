using Application.Dtos.Contract;
using Application.Dtos.Pagination;
using Domain.Specification.Params;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Contracts.Queries.GetAll
{
    public class GetAllContractsQuery : IRequest<PaginationDTO<ContractDto>>
    {
        public ContractPaginationParams PaginationParams { get; set; }
        public GetAllContractsQuery(ContractPaginationParams p) => PaginationParams = p;
    }
}
