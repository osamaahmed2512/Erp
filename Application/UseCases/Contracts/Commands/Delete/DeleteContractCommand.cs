using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Contracts.Commands.Delete
{
    public class DeleteContractCommand : IRequest<BaseApiResponse>
    {
        public Guid ContractId { get; set; }
    }
}
