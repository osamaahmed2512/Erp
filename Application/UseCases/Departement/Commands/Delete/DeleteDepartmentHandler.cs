using Application.Dtos.Response;
using Application.Interfaces.InternalServices;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.Departement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Departement.Commands.Delete
{
    public class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _userService;
        public DeleteDepartmentHandler(IUnitOfWork unitOfWork , ICurrentUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService= userService;
        }

        public async Task<BaseApiResponse> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {

            var spec =new DepartmentSpecification(request.DepartmentId);
            var department = await _unitOfWork.Repository<Domain.Entities.Department>().GetByIdSpecAsync(spec);

            if (department is null)
                return BaseApiResponse.Fail(404, "Department not found.");

            if (department.Company.OwnerId != _userService.UserId &&!_userService.IsSuperAdmin)
                return BaseApiResponse.Fail(403, "You are not authorized to delete this department.");

                department.Status = Domain.Enum.EntityStatus.Deleted;
                department.UpdatedAt = DateTime.UtcNow;


            await _unitOfWork.SaveChangeAsync(cancellationToken);

            return BaseApiResponse.Success(200, "Department deleted successfully.");
        }
    }
}
