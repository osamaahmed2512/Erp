using Application.Dtos.Response;
using Application.Interfaces.InternalServices;
using Domain.Interfaces.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Commands.Delete
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, BaseApiResponse>
    { 
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;
        public DeleteCompanyCommandHandler(IUnitOfWork unitOfWork , ICurrentUserService currentUserService)
        {
            _uow=unitOfWork;
            _currentUserService=currentUserService;
        }
        public async Task<BaseApiResponse> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {

            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(request.companyId);

            if (company is null)
                return BaseApiResponse.Fail(404, "Employee not found.");

            company.Status= Domain.Enum.EntityStatus.Deleted;   
            await _uow.SaveChangeAsync(cancellationToken);

            return BaseApiResponse.Success(200, "Employee deleted successfully.");
        }
    }
}
