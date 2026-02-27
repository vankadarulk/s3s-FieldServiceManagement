using FieldServiceManagement.API.Data;
using FieldServiceManagement.API.Services;
using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Enums;
using FieldServiceManagement.Core.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FieldServiceManagement.API.Tests.Services;

public class JobServiceTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByTechnicianIdAsync_ReturnsOnlyAssignedJobs()
    {
        var context = CreateInMemoryContext(Guid.NewGuid().ToString());
        var techId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        context.Jobs.AddRange(
            new Job { Id = Guid.NewGuid(), Title = "Job1", AssignedTechnicianId = techId },
            new Job { Id = Guid.NewGuid(), Title = "Job2", AssignedTechnicianId = otherId },
            new Job { Id = Guid.NewGuid(), Title = "Job3", AssignedTechnicianId = techId }
        );
        await context.SaveChangesAsync();

        var service = new JobService(context);
        var result = await service.GetByTechnicianIdAsync(techId);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(j => j.AssignedTechnicianId == techId);
    }

    [Fact]
    public async Task CreateAsync_CreatesJobWithCorrectStatus()
    {
        var context = CreateInMemoryContext(Guid.NewGuid().ToString());
        var techId = Guid.NewGuid();
        var service = new JobService(context);

        var dto = new JobCreateDto
        {
            Title = "New Job",
            Priority = JobPriority.High,
            AssignedTechnicianId = techId
        };

        var result = await service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Title.Should().Be("New Job");
        result.Status.Should().Be(JobStatus.Assigned);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesJob()
    {
        var context = CreateInMemoryContext(Guid.NewGuid().ToString());
        var jobId = Guid.NewGuid();
        context.Jobs.Add(new Job { Id = jobId, Title = "ToDelete" });
        await context.SaveChangesAsync();

        var service = new JobService(context);
        var result = await service.DeleteAsync(jobId);

        result.Should().BeTrue();
        var deletedJob = await context.Jobs.IgnoreQueryFilters().FirstOrDefaultAsync(j => j.Id == jobId);
        deletedJob!.IsDeleted.Should().BeTrue();
    }
}
