using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Enums;
using FieldServiceManagement.Core.Interfaces;
using FieldServiceManagement.MauiApp.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace FieldServiceManagement.MauiApp.Tests.ViewModels;

public class LoginViewModelTests
{
    [Fact]
    public async Task LoginAsync_WithValidCredentials_SetsIsLoggedIn()
    {
        var mockAuth = new Mock<IAuthService>();
        mockAuth.Setup(a => a.LoginAsync(It.IsAny<LoginRequestDto>()))
            .ReturnsAsync(new LoginResponseDto
            {
                Token = "test-token",
                RefreshToken = "refresh-token",
                Expiry = DateTime.UtcNow.AddHours(1),
                User = new UserDto { Id = Guid.NewGuid(), Email = "admin@test.com", Role = UserRole.Admin }
            });

        var vm = new LoginViewModel(mockAuth.Object)
        {
            Email = "admin@test.com",
            Password = "password"
        };

        await vm.LoginAsync();

        vm.IsLoggedIn.Should().BeTrue();
        vm.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_SetsErrorMessage()
    {
        var mockAuth = new Mock<IAuthService>();
        mockAuth.Setup(a => a.LoginAsync(It.IsAny<LoginRequestDto>()))
            .ReturnsAsync((LoginResponseDto?)null);

        var vm = new LoginViewModel(mockAuth.Object)
        {
            Email = "wrong@test.com",
            Password = "wrong"
        };

        await vm.LoginAsync();

        vm.IsLoggedIn.Should().BeFalse();
        vm.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_WithEmptyEmail_SetsErrorMessage()
    {
        var mockAuth = new Mock<IAuthService>();
        var vm = new LoginViewModel(mockAuth.Object)
        {
            Email = string.Empty,
            Password = "password"
        };

        await vm.LoginAsync();

        vm.IsLoggedIn.Should().BeFalse();
        vm.ErrorMessage.Should().NotBeNullOrEmpty();
        mockAuth.Verify(a => a.LoginAsync(It.IsAny<LoginRequestDto>()), Times.Never);
    }
}
