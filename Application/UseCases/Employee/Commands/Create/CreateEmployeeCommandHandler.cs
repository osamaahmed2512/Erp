using Application.Dtos.Employee;
using Application.Dtos.Response;
using Application.Interfaces.ExternalServices;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Employee;
using MediatR;


namespace Application.UseCases.Employee.Commands.Create
{
    public class CreateEmployeeCommandHandler
        : IRequestHandler<CreateEmployeeCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly IIdentityService _identityService;

        public CreateEmployeeCommandHandler(IUnitOfWork uow, IIdentityService identityService)
        {
            _uow = uow;
            _identityService = identityService;
        }

        public async Task<BaseApiResponse> Handle(
        CreateEmployeeCommand request, CancellationToken cancellationToken)
        { 
            int nextNumber = 1;
           var lastEmployeeCode=  _uow.Repository<Domain.Entities.Employee>().GetQueryableWithSpec(null)
                .Where(x =>x.CompanyId ==request.CompanyId)
                .OrderByDescending(x =>x.EmpCode).FirstOrDefault();
            var company = await _uow.Repository<Domain.Entities.Company>().GetByIdAsync(request.CompanyId);
            if (lastEmployeeCode != null) 
            {
               var sequence = lastEmployeeCode.EmpCode[^5..];
                nextNumber = int.Parse(sequence) + 1;
                
            }
           
            var dto = request.Dto;
            var existEmail = await _identityService.FindByEmailAsync(dto.Email);
            if (existEmail != null)
                return BaseApiResponse.Fail(400, "Email Is Already Exist");
            var existPhone = await _identityService.FindByPhoneAsync(dto.phone);
            if (existPhone != null)
                return BaseApiResponse.Fail(400, "Phone Nmuber Is Already Exist");
            var userId = await _identityService.CreateUserAsync(
                email: dto.Email,
                password: dto.Password,
                Role: SystemRoles.Employee.ToString(),
                firstName: dto.FirstName,
                lastName: dto.LastName,
                phone:dto.phone
            );

            var employee = new Domain.Entities.Employee
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Status=EmployeeStatus.Created,
                Address= dto.Address,
                NationalityId = dto.NationalityId,
                Gender=dto.Gender,
                MartielStatus=dto.MartielStatus,
                NationalityNumber=dto.NationalityNumber,
                BirthDate=dto.BirthDate,
                CompanyId=request.CompanyId,
                EmpCode=$"{company.CompanyCode:D4}{nextNumber:D5}"
            };

            await _uow.Repository<Domain.Entities.Employee>().AddAsync(employee);
            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(201, "Employee created successfully.");
        }
    }
}
