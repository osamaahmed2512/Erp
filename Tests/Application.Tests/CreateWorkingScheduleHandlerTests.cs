using Application.Dtos.WorkingSchedule;
using Application.Tests.Fakes;
using Application.UseCases.WorkingSchedule.Commands.Create;

namespace Application.Tests.WorkingSchedule;

public sealed class CreateWorkingScheduleHandlerTests
{
    [Fact]
    public async Task Handle_RejectsMissingResolvedCompanyContext()
    {
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateWorkingScheduleHandler(unitOfWork);

        var response = await handler.Handle(new CreateWorkingScheduleCommand(
            new CreateWorkingScheduleDto
            {
                Name = "Standard",
                CompanyId = null,
                EffectiveFrom = DateTime.UtcNow,
                Days = []
            },
            Guid.NewGuid(),
            true), CancellationToken.None);

        Assert.Equal(400, response.StatusCode);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Handle_CreatesScheduleForResolvedCompany()
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
        var handler = new CreateWorkingScheduleHandler(unitOfWork);

        var response = await handler.Handle(new CreateWorkingScheduleCommand(
            new CreateWorkingScheduleDto
            {
                Name = "Standard",
                CompanyId = companyId,
                EffectiveFrom = DateTime.UtcNow,
                Days = []
            },
            Guid.NewGuid(),
            true), CancellationToken.None);

        var schedules = await unitOfWork.Repository<Domain.Entities.WorkingSchedule>()
            .GetAllAsync(CancellationToken.None);
        Assert.Equal(201, response.StatusCode);
        Assert.Single(schedules);
        Assert.Equal(companyId, schedules[0].CompanyId);
        Assert.Equal(1, unitOfWork.SaveCount);
    }
}
