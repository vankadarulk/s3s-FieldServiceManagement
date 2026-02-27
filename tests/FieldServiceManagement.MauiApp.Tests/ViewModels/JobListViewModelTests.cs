using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Enums;
using FieldServiceManagement.MauiApp.Services;
using FieldServiceManagement.MauiApp.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace FieldServiceManagement.MauiApp.Tests.ViewModels;

public class JobListViewModelTests
{
    [Fact]
    public async Task LoadJobsAsync_PopulatesJobsCollection()
    {
        var mockApi = new Mock<IApiService>();
        var mockConnectivity = new Mock<IConnectivityService>();
        mockConnectivity.Setup(c => c.IsConnected).Returns(true);
        mockApi.Setup(a => a.GetJobsAsync()).ReturnsAsync(new List<JobDto>
        {
            new() { Id = Guid.NewGuid(), Title = "Job A", Status = JobStatus.Created },
            new() { Id = Guid.NewGuid(), Title = "Job B", Status = JobStatus.InProgress }
        });

        var vm = new JobListViewModel(mockApi.Object, mockConnectivity.Object);

        await vm.LoadJobsAsync();

        vm.Jobs.Should().HaveCount(2);
        vm.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task LoadJobsAsync_WhenApiThrows_SetsErrorMessage()
    {
        var mockApi = new Mock<IApiService>();
        var mockConnectivity = new Mock<IConnectivityService>();
        mockApi.Setup(a => a.GetJobsAsync()).ThrowsAsync(new Exception("Network error"));

        var vm = new JobListViewModel(mockApi.Object, mockConnectivity.Object);

        await vm.LoadJobsAsync();

        vm.Jobs.Should().BeEmpty();
        vm.ErrorMessage.Should().NotBeNullOrEmpty();
    }
}
