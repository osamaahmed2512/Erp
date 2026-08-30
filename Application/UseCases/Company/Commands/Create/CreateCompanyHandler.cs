using Application.Dtos.Response;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using MediatR;

namespace Application.UseCases.Company.Commands.Create;

[Obsolete("Use CreateCompanyWithOwnerCommandHandler. This handler is not exposed by an API endpoint.")]
public sealed class CreateCompanyHandler : IRequestHandler<CreateComapnyCommand, BaseApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateCompanyHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BaseApiResponse> Handle(CreateComapnyCommand request, CancellationToken cancellationToken)
    {
        var dto = request.dto;
        var spec = new CompanySpecification(dto.Phone, dto.Email);
        var existing = await _unitOfWork.Repository<Domain.Entities.Company>()
            .GetSingleProjectedAsync(x => new { x.Phone, x.Email }, spec, cancellationToken);
        if (existing is not null)
        {
            if (existing.Phone == dto.Phone) return BaseApiResponse.Fail(400, "Phone number already exists.");
            if (existing.Email == dto.Email) return BaseApiResponse.Fail(400, "Email already exists.");
        }

        var company = new Domain.Entities.Company
        {
            Name = dto.Name, Email = dto.Email, Phone = dto.Phone, Description = dto.Description,
            Country = dto.Country, City = dto.City, Address = dto.Address, PostalCode = dto.PostalCode,
            TaxNumber = dto.TaxNumber, CommercialRegistration = dto.CommercialRegistration,
            Website = dto.Website, OwnerId = request.OwnerId
        };
        var owner = await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(request.OwnerId, cancellationToken);
        if (owner is null || owner.IsRootSuperAdmin || owner.CompanyId.HasValue)
            return BaseApiResponse.Fail(409, "Owner account is invalid or already assigned to a company.");
        owner.AccountType = AccountType.Company;
        owner.CompanyId = company.Id;
        await _unitOfWork.Repository<Domain.Entities.Company>().AddAsync(company, cancellationToken);
        await _unitOfWork.Repository<ApplicationUser>().UpdateAsync(owner);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return BaseApiResponse.Success(201, "Company created successfully.");
    }
}
