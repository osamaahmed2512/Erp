using Application.Dtos.DropDown;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Contracts.Queries.GetDropDown
{
    public class GetContractDropDownQuery : IRequest<List<DropDownDto>>
    {
        public Guid? EmployeeId { get; set; }
    }
}
