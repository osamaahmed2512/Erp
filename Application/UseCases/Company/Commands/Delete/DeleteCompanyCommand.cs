using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Commands.Delete
{
    public class DeleteCompanyCommand:IRequest<BaseApiResponse>
    {
        public Guid companyId { get; set; }
    }
}
