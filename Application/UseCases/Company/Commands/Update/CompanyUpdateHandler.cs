using Application.Dtos.Response;
using Application.Interfaces.InternalServices;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using MediatR;


namespace Application.UseCases.Company.Commands.Update
{
    public class CompanyUpdateHandler : IRequestHandler<CompanyUpdateCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;
        public CompanyUpdateHandler(IUnitOfWork unitOfWork,ICurrentUserService currentUserService)
        {
            _uow = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<BaseApiResponse> Handle(CompanyUpdateCommand request, CancellationToken cancellationToken)
        {
            var dto = request.dto;
            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(request.companyId);
            if (company == null) 
                return BaseApiResponse.Fail(404, "Company not found.");
            if (company.OwnerId != request.ownerId && !_currentUserService.IsSuperAdmin)
                return BaseApiResponse.Fail(403, "You are not allowed to update this company.");

            var spec = new CompanySpecification(dto.Phone, dto.Email,company.Id);
                var existCompany = await _uow.Repository<Domain.Entities.Company>()
                    .GetSingleProjectedAsync(x => new { x.Phone, x.Email }, spec);
            if (existCompany != null &&!_currentUserService.IsSuperAdmin)
            {
                if (existCompany.Phone == dto.Phone)
                {
                    return BaseApiResponse.Fail(400, "Phone number already exists.");
                }
                else
                {
                    return BaseApiResponse.Fail(400, "Email already exists.");
                }
            }
            company.Name = dto.Name;
            company.Email = dto.Email;
            company.Phone = dto.Phone;
            company.City = dto.City;
            company.Country = dto.Country;  
            company.Address = dto.Address;
            company.PostalCode = dto.PostalCode;    
            company.TaxNumber= dto.TaxNumber;
            company.CommercialRegistration = dto.CommercialRegistration;
            company.Website = dto.Website;
            company.Description = dto.Description;
            company.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangeAsync(cancellationToken);

            return BaseApiResponse.Success(200, "Company updated successfully.");
        }
    }
}
