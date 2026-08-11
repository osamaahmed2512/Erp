using Application.Dtos.Company;
using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Commands.Create
{
    public class CreateComapnyCommand:IRequest<BaseApiResponse>
    {
        public CreateCompanyDto dto { get; set; }
        public Guid OwnerId { get; set; }

    }
}
