using Application.Dtos.Company;
using Application.Tests.Fakes;
using Application.UseCases.Company.Commands.Create;

namespace Application.Tests.Company;

public sealed class CreateCompanyHandlerTests
{
    [Fact]
    public async Task Handle_RejectsMatchingCompany_WhenMatchIsNotFirstRecord()
    {
        var unitOfWork = new FakeUnitOfWork().Seed(
            new Domain.Entities.Company { Name = "First", Email = "first@test.com", Phone = "100" },
            new Domain.Entities.Company { Name = "Duplicate", Email = "duplicate@test.com", Phone = "200" });
        var handler = new CreateCompanyHandler(unitOfWork);
        var command = new CreateComapnyCommand
        {
            OwnerId = Guid.NewGuid(),
            dto = new CreateCompanyDto
            {
                Name = "New company",
                Email = "duplicate@test.com",
                Phone = "999"
            }
        };

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(400, response.StatusCode);
        Assert.Equal("Email already exists.", response.Message);
        Assert.Equal(0, unitOfWork.SaveCount);
    }
}
