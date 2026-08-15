using Application.Dtos.Response;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Employee.Commands.UploadProfilePhoto
{
    public class UploadEmployeeProfilePhotoCommand : IRequest<BaseApiResponse<string>>
    {
        public Guid EmployeeId { get; set; }
        public IFormFile Photo { get; set; } = null!;
    }
}
