using Application.Dtos.Company;
using Application.Dtos.Response;
using Application.Interfaces.InternalServices;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Company;
using MediatR;



namespace Application.UseCases.Company.Queries.GetById
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, BaseApiResponse<CompanyDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public GetCompanyByIdQueryHandler(IUnitOfWork unitOfWork,ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<BaseApiResponse<CompanyDetailsDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var compnay = await _unitOfWork.Repository<Domain.Entities.Company>()
                .GetByIdSpecAsync(new CompanySpecification(request.Id, true));
            if (compnay == null)
            {
                return BaseApiResponse<CompanyDetailsDto>.Fail(404, "Company not found");
            }
            if (!_currentUserService.IsSuperAdmin && _currentUserService.UserId != compnay.OwnerId)
            {
                return BaseApiResponse<CompanyDetailsDto>.Fail(403, "You are not allowed to access this company");
            }
            var data = new CompanyDetailsDto
            {
                Id = compnay.Id,
                Name = compnay.Name,
                Email = compnay.Email,
                Phone = compnay.Phone,
                OwnerName = compnay.Owner.FirstName + " " + compnay.Owner.LastName,
                Status = compnay.Status.ToString(),
                Website = compnay.Website,
                Address = compnay.Address,
                City = compnay.City,
                Country = compnay.Country,
                CommercialRegistration = compnay.CommercialRegistration,
                Description = compnay.Description,
                PostalCode = compnay.PostalCode,
                TaxNumber = compnay.TaxNumber
            };
            return new BaseApiResponse<CompanyDetailsDto>
            {
                Data = data,
                Message = "Company retrieved successfully",
                StatusCode = 200
            };
        }
    }
}
