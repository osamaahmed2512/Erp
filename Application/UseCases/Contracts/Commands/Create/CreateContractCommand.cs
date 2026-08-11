using Application.Dtos.Contract;
using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Contracts.Commands.Create
{
    public class CreateContractCommand : IRequest<BaseApiResponse>
    {
        public CreateContractDto Dto { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
