using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.Departement.Commands.Delete
{
    public class DeleteDepartmentCommand : IRequest<BaseApiResponse>
    {
        public Guid DepartmentId { get; set; }
    }
}
