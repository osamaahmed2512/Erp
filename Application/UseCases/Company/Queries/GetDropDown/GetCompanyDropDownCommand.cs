using Application.Dtos.DropDown;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Queries.GetDropDown
{
    public class GetCompanyDropDownCommand : IRequest<List<DropDownDto>>
    {
        public Guid? OwnerId { get; set; }
    }
}
