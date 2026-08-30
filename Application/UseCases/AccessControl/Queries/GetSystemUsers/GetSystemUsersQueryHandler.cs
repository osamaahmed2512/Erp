using Application.Dtos.AccessControl;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using Domain.Specification.AccessControl;
using MediatR;

namespace Application.UseCases.AccessControl.Queries.GetSystemUsers;

public sealed class GetSystemUsersQueryHandler : IRequestHandler<GetSystemUsersQuery, IReadOnlyList<AccessUserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetSystemUsersQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<AccessUserDto>> Handle(GetSystemUsersQuery request, CancellationToken cancellationToken)
    {
        var spec = new AccessUserSearchSpecification(request.Search?.Trim().ToLowerInvariant(), AccountType.System);
        return await _unitOfWork.Repository<ApplicationUser>().GetProjectedAsync(x =>
            new AccessUserDto(x.Id, x.Email ?? string.Empty, (x.FirstName + " " + x.LastName).Trim(),
                x.AccountType, x.IsRootSuperAdmin, false), spec, cancellationToken);
    }
}
