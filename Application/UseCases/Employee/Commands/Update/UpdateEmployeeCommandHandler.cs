using Application.Dtos.Response;
using Application.Interfaces.ExternalServices;
using Domain.Entities;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Employee;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Employee.Commands.Update
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly IIdentityService _identityService;
        public UpdateEmployeeCommandHandler(IUnitOfWork uow , IIdentityService IdentityService)
        {
            _uow = uow;
            _identityService = IdentityService;
        }


        public async Task<BaseApiResponse> Handle(
            UpdateEmployeeCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Request;

            var employee = await _uow.Repository<Domain.Entities.Employee>()
                .GetByIdAsync(request.Id);

            if (employee is null)
                return BaseApiResponse.Fail(404, "Employee not found.");

            var existingEmail = await _identityService.FindByEmailAsync(dto.Email);

            if (existingEmail != null && existingEmail.Id != employee.UserId)
            {
                return BaseApiResponse.Fail(400, "Email is already exists.");
            }

            var existingPhone = await _identityService.FindByPhoneAsync(dto.phone);

            if (existingPhone != null && existingPhone.Id != employee.UserId)
            {
                return BaseApiResponse.Fail(400, "Phone is already exists.");
            }

            employee.User.FirstName = dto.FirstName;
            employee.User.LastName = dto.LastName;
            employee.User.PhoneNumber = dto.phone;

            if (employee.User.Email != dto.Email)
            {
                await _identityService.ChangeEmailAsync(
                    employee.UserId,
                    dto.Email
                );
            }


            if (!string.IsNullOrWhiteSpace(dto.newPassword))
            {
                await _identityService.ChangePasswordAsync(
                    employee.UserId,
                    dto.newPassword
                );
            }

            employee.NationalityNumber = dto.NationalityNumber;
            employee.NationalityId = dto.NationalityId;
            employee.Address = dto.Address;
            employee.MartielStatus = dto.MartielStatus;
            employee.BirthDate = dto.BirthDate;
            employee.Gender = dto.Gender;

            await _uow.Repository<Domain.Entities.Employee>()
                .UpdateAsync(employee);

            await _uow.SaveChangeAsync(cancellationToken);

            return BaseApiResponse.Success(
                200,
                "Employee updated successfully."
            );
        }
    }
}
