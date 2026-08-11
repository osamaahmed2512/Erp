using Application.Dtos.DropDown;
using Application.Dtos.Nationality;
using Application.Dtos.Pagination;
using Application.Dtos.Response;
using Domain.Specification.Params;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Nationality.Queries.GetAll
{
    public record GetAllNationalitiesQuery()
        :IRequest<List<DropDownDto>>
    {
    }
}
