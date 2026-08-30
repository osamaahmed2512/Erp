using Application.Dtos.Response;
using Application.Interfaces.ExternalServices;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using MediatR;

namespace Application.UseCases.AccessControl.Commands.CreateSystemUser;

public sealed class CreateSystemUserCommandHandler :
    IRequestHandler<CreateSystemUserCommand, BaseApiResponse<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    public CreateSystemUserCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<BaseApiResponse<Guid>> Handle(CreateSystemUserCommand request, CancellationToken cancellationToken)
    {
        if (!await CanCreateAsync(request.ActorId, cancellationToken))
            return new(403, "You do not have permission to create system users.");
        if (await _identityService.FindByEmailAsync(request.Dto.Email.Trim()) is not null)
            return new(409, "Email already exists.");
        if (!string.IsNullOrWhiteSpace(request.Dto.Phone) &&
            await _identityService.FindByPhoneAsync(request.Dto.Phone.Trim()) is not null)
            return new(409, "Phone number already exists.");
        try
        {
            var id = await _identityService.CreateUserAsync(
                request.Dto.Email.Trim(), request.Dto.Password, SystemRoles.Admin.ToString(),
                request.Dto.FirstName.Trim(), request.Dto.LastName.Trim(), request.Dto.Phone?.Trim(),
                AccountType.System);
            return new BaseApiResponse<Guid>(201, "System user created successfully.", id);
        }
        catch (Exception ex)
        {
            return new BaseApiResponse<Guid>(400, ex.Message);
        }
    }

    private async Task<bool> CanCreateAsync(Guid actorId, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(actorId, cancellationToken);
        if (user?.IsRootSuperAdmin == true) return true;
        if (user?.AccountType != AccountType.System) return false;
        return await _unitOfWork.Repository<SystemUserRole>().AnyAsync(x =>
            x.UserId == actorId && x.SystemRole.Permissions.Any(p =>
                p.PermissionDefinition.Key == "SystemUsers.Create"), cancellationToken) &&
            !await _unitOfWork.Repository<UserPermissionOverride>().AnyAsync(x =>
                x.UserId == actorId && x.PermissionDefinition.Key == "SystemUsers.Create", cancellationToken);
    }
}
