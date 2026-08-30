using Application.Dtos.Departement;
using Application.Tests.Fakes;
using Application.UseCases.Departement.Commands.Create;

namespace Application.Tests.Department;

public sealed class CreateDepartmentHandlerTests
{
    [Fact]
    public async Task Handle_RejectsMissingResolvedCompanyContext()
    {
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateDepartmentHandler(unitOfWork);

        var response = await handler.Handle(new CreateDepartmentCommand
        {
            OwnerId = Guid.NewGuid(),
            Dto = new CreateDepartmentDto { Name = "HR", CompanyId = null }
        }, CancellationToken.None);

        Assert.Equal(400, response.StatusCode);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Handle_CreatesDepartmentForResolvedCompany()
    {
        var companyId = Guid.NewGuid();
        var unitOfWork = new FakeUnitOfWork()
            .Seed(new Domain.Entities.Company
            {
                Id = companyId,
                Name = "ERP",
                Email = "erp@test.com",
                Phone = "123"
            });
        var handler = new CreateDepartmentHandler(unitOfWork);

        var response = await handler.Handle(new CreateDepartmentCommand
        {
            OwnerId = Guid.NewGuid(),
            Dto = new CreateDepartmentDto { Name = "HR", CompanyId = companyId }
        }, CancellationToken.None);

        var departments = await unitOfWork.Repository<Domain.Entities.Department>()
            .GetAllAsync(CancellationToken.None);
        Assert.Equal(201, response.StatusCode);
        Assert.Single(departments);
        Assert.Equal(companyId, departments[0].CompanyId);
        Assert.Equal(1, unitOfWork.SaveCount);
    }
}
