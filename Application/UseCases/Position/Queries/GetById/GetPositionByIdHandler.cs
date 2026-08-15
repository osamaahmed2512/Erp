using Application.Dtos.Position;
using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Queries.GetById
{
    public class GetPositionByIdHandler : IRequestHandler<GetPositionByIdQuery, BaseApiResponse<PositionDetailsDto>>
    {
        private readonly IUnitOfWork _uow;
        public GetPositionByIdHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse<PositionDetailsDto>> Handle(GetPositionByIdQuery request, CancellationToken cancellationToken)
        {
            var position = await _uow.Repository<Domain.Entities.Position>().GetByIdAsync(request.Id);
            if (position is null || position.Status == Domain.Enum.EntityStatus.Deleted)
                return BaseApiResponse<PositionDetailsDto>.Fail(404, "Position not found.");

            var department = await _uow.Repository<Domain.Entities.Department>().GetByIdAsync(position.DepartmentId);
            if (department is null)
                return BaseApiResponse<PositionDetailsDto>.Fail(404, "Position department not found.");

            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(department.CompanyId);
            if (company is null)
                return BaseApiResponse<PositionDetailsDto>.Fail(404, "Department company not found.");

            if (!request.IsAdmin && company.OwnerId != request.OwnerId)
                return BaseApiResponse<PositionDetailsDto>.Fail(403, "You are not allowed to access this position.");

            var data = new PositionDetailsDto
            {
                Id = position.Id,
                Title = position.Title,
                Description = position.Description,
                DepartmentId = position.DepartmentId,
                DepartmentName = department.Name,
                CompanyId = department.CompanyId,
                CompanyName = company.Name,
                Status = position.Status.ToString(),
                TotalEmployees = 0,
                CreatedAt = position.CreatedAt,
                UpdatedAt = position.UpdatedAt
            };

            return new BaseApiResponse<PositionDetailsDto>
            {
                Data = data,
                Message = "Position retrieved successfully.",
                StatusCode = 200
            };
        }
    }
}
