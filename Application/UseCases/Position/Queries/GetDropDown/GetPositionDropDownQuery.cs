using Application.Dtos.DropDown;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Queries.GetDropDown
{
    public class GetPositionDropDownQuery : IRequest<List<DropDownDto>>
    {
        public Guid? DepartmentId { get; set; }
    }
}
