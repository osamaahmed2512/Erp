using Application.Dtos.DropDown;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Queries.GetDropDown
{
    public class GetDepartmentDropDownQuery : IRequest<List<DropDownDto>>
    {
        public Guid? CompanyId { get; set; }
    }
}
