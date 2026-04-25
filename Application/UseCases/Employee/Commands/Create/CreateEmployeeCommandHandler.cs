using Application.Dtos.Employee;
using Application.Dtos.Response;
using Application.Interfaces.ExternalServices;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var dto = request.Dto;
            var userId = await _identityService.CreateUserAsync(
                email: dto.Email,
                password: dto.Password,
                Role: SystemRoles.Employee.ToString(),
                firstName: dto.FirstName,
                lastName: dto.LastName
            );

            var employee = new Domain.Entities.Employee
            {
                Id = Guid.NewGuid(),
                UserId = userId
            };

            await _uow.Repository<Domain.Entities.Employee>().AddAsync(employee);
            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(201, "Employee created successfully.");
        }
    }
}
