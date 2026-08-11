using Application.Dtos.Response;
using Application.Interfaces.InternalServices;
using Domain.Interfaces.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Commands.Status
{
    public class CompanyUpdateStatuscommandHandler : IRequestHandler<CompanyUpdateStatuscommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;
        public CompanyUpdateStatuscommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _uow = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<BaseApiResponse> Handle(CompanyUpdateStatuscommand request, CancellationToken cancellationToken)
        {
            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(request.Id);

            if (company is null)
                return BaseApiResponse.Fail(404, "Company not found.");
            if (!_currentUserService.IsAdmin && company.OwnerId != _currentUserService.UserId)
                return BaseApiResponse.Fail(403, "You are not allowed to Access this company.");
            if (request.status.ToLower() == "active")
            {
                company.Status = Domain.Enum.EntityStatus.Active;
            }
            else
            {
                company.Status = Domain.Enum.EntityStatus.Inactive;
            }
           
            await _uow.SaveChangeAsync(cancellationToken);

            return BaseApiResponse.Success(200, $"company {company.Status} successfully.");
        }
    }
}
