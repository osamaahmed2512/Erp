using Application.Dtos.Response;
using Domain.Enum;
using Domain.Interfaces.UnitOfWork;
using MediatR;

namespace Application.UseCases.Contracts.Commands.Delete
{
    public class DeleteContractHandler : IRequestHandler<DeleteContractCommand, BaseApiResponse>
    {
        private readonly IUnitOfWork _uow;
        public DeleteContractHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<BaseApiResponse> Handle(DeleteContractCommand request, CancellationToken cancellationToken)
        {
            var contract = await _uow.Repository<Domain.Entities.Contract>().GetByIdAsync(request.ContractId);
            if (contract is null || contract.Status == ContractStatus.Terminated)
                return BaseApiResponse.Fail(404, "Contract not found.");

            contract.Status = ContractStatus.Terminated;
            contract.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangeAsync(cancellationToken);
            return BaseApiResponse.Success(200, "Contract terminated successfully.");
        }
    }
}
