using Application.Dtos.Response;
using Application.Helpers.Upload;
using Domain.Interfaces.UnitOfWork;
using MediatR;

namespace Application.UseCases.Employee.Commands.UploadProfilePhoto
{
    public class UploadEmployeeProfilePhotoHandler : IRequestHandler<UploadEmployeeProfilePhotoCommand, BaseApiResponse<string>>
    {
        private readonly IUnitOfWork _uow;
        public UploadEmployeeProfilePhotoHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<BaseApiResponse<string>> Handle(UploadEmployeeProfilePhotoCommand request, CancellationToken cancellationToken)
        {
            var employee = await _uow.Repository<Domain.Entities.Employee>().GetByIdAsync(request.EmployeeId);
            if (employee is null || employee.IsDeleted)
                return BaseApiResponse<string>.Fail(404, "Employee not found.");

            var upload = DocumentSettings.UpdateProfilePhoto(request.Photo, employee.ProfilePhoto);
            if (upload.Response.StatusCode >= 400)
                return BaseApiResponse<string>.Fail(upload.Response.StatusCode, upload.Response.Message);

            employee.ProfilePhoto = upload.StoredFileName;
            employee.UpdatedAt = DateTime.UtcNow;
            await _uow.SaveChangeAsync(cancellationToken);
            return new BaseApiResponse<string>(200, "Profile photo uploaded successfully.", $"/pic/employees/{upload.StoredFileName}");
        }
    }
}
