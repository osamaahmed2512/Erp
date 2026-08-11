using Application.Dtos.Company;
using Application.Dtos.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Commands.Update
{
    public class CompanyUpdateCommand:IRequest<BaseApiResponse>
    {
        public UpdateCompanyDto dto { get; set; }
        public Guid ownerId { get; set; }
        public Guid companyId { get; set; }
    }
}
