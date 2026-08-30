using Application.Dtos.Company;
using Application.Dtos.Response;
using MediatR;

namespace Application.UseCases.Company.Commands.Create;

[Obsolete("Use CreateCompanyWithOwnerCommand. Kept only for compatibility with existing application tests.")]
public sealed class CreateComapnyCommand : IRequest<BaseApiResponse>
{
    public CreateCompanyDto dto { get; set; } = null!;
    public Guid OwnerId { get; set; }
}
