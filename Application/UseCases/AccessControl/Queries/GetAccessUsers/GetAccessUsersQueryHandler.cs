using Application.Dtos.AccessControl;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetAccessUsers;

public sealed class GetAccessUsersQueryHandler : IRequestHandler<GetAccessUsersQuery, IReadOnlyList<AccessUserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAccessUsersQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<AccessUserDto>> Handle(GetAccessUsersQuery request, CancellationToken cancellationToken)
    {
        var spec = new AccessUserSearchSpecification(
            request.Search?.Trim().ToLowerInvariant(), AccountType.Company, request.CompanyId);
        return await _unitOfWork.Repository<ApplicationUser>().GetProjectedAsync(x =>
            new AccessUserDto(x.Id, x.Email ?? string.Empty, (x.FirstName + " " + x.LastName).Trim(),
                x.AccountType, x.IsRootSuperAdmin, x.OwnedCompanies.Any(c => c.Id == request.CompanyId)),
            spec, cancellationToken);
    }
}
