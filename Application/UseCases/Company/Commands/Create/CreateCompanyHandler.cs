using Application.Dtos.Response;
using Application.Interfaces.ExternalServices;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Company.Commands.Create
{
    public class CreateCompanyHandler : IRequestHandler<CreateComapnyCommand, BaseApiResponse>
    { 
        private readonly IUnitOfWork _unitOfWork;
        public CreateCompanyHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task<BaseApiResponse> Handle(CreateComapnyCommand request, CancellationToken cancellationToken)
        {
            var dto = request.dto;

            var spec = new CompanySpecification(dto.Phone, dto.Email);
            var existCompany =await _unitOfWork.Repository<Domain.Entities.Company>()
                .GetSingleProjectedAsync(x => new { x.Phone ,x.Email});
            if (existCompany != null) 
            {
                if (existCompany.Phone == dto.Phone)
                {
                    return BaseApiResponse.Fail(400, "Phone number already exists.");
                }
                if (existCompany.Email == dto.Email)
                {
                    return BaseApiResponse.Fail(400, "Email already exists.");
                }
            }

            var company = new Domain.Entities.Company
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Description = dto.Description,
                    Country = dto.Country,
                    City = dto.City,
                    Address = dto.Address,
                    PostalCode = dto.PostalCode,
                    TaxNumber = dto.TaxNumber,
                    CommercialRegistration = dto.CommercialRegistration,
                    Website = dto.Website,
                    OwnerId = request.OwnerId,
               };


            await _unitOfWork.Repository<Domain.Entities.Company>().AddAsync(company);
            await _unitOfWork.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(201, "Company created successfully.");
        }
    }
}
