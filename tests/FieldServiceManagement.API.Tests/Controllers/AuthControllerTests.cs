using System.Net;
using System.Net.Http.Json;
using FieldServiceManagement.Core.DTOs;
using FluentAssertions;
using Xunit;

namespace FieldServiceManagement.API.Tests.Controllers;

public class AuthControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsJwtToken()
    {
        var request = new LoginRequestDto
        {
            Email = "admin@fieldservice.com",
            Password = "Admin@123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        var request = new LoginRequestDto
        {
            Email = "wrong@example.com",
            Password = "wrongpassword"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
