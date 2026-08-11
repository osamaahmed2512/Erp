using Application.Dtos.Response;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using Domain.Specification.Position;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Position.Commands.Create
{
    public class CreatePositionHandler : IRequestHandler<CreatePositionCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        public CreatePositionHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var title = dto.Title.Trim();
            var companySpec = new CompanySpecification(dto.CompanyId);
            var company = await _uow.Repository<Domain.Entities.Company>().GetSingleProjectedAsync(c => new
            {
                c.OwnerId
            }, companySpec);

            if (company is null)
                return BaseApiResponse.Fail(404, "Company not found.");

            if (company.OwnerId != request.OwnerId)
                return BaseApiResponse.Fail(403, "You are not allowed to add positions to this company.");

            var spec = new PositionSpecification(title, dto.CompanyId);
            var exists = await _uow.Repository<Domain.Entities.Position>()
                .GetSingleProjectedAsync(x => new { x.Id }, spec);

            if (exists is not null)
                return BaseApiResponse.Fail(400, "A position with this title already exists in the company.");

            var position = new Domain.Entities.Position
            {
                Title = title,
                Description = dto.Description?.Trim(),
                CompanyId = dto.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Repository<Domain.Entities.Position>().AddAsync(position);
            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(201, "Position created successfully.");
        }
    }
}
