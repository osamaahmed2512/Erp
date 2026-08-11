using Application.Dtos.Response;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Commands.Status
{
    public class CompanyUpdateStatuscommand:IRequest<BaseApiResponse>
    {
        public Guid Id { get; set; }
        public string status { get; set; }
    }
}
