using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Enums;
using FieldServiceManagement.Core.Validators;
using FluentAssertions;
using Xunit;

namespace FieldServiceManagement.Core.Tests.Validators;

public class UserCreateValidatorTests
{
    private readonly UserCreateValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Email_Is_Empty()
    {
        var dto = new UserCreateDto { Email = string.Empty, FirstName = "John", LastName = "Doe", Password = "Password1!", Role = UserRole.Technician };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Should_Fail_When_Email_Is_Invalid()
    {
        var dto = new UserCreateDto { Email = "not-an-email", FirstName = "John", LastName = "Doe", Password = "Password1!", Role = UserRole.Technician };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Should_Fail_When_FirstName_Is_Empty()
    {
        var dto = new UserCreateDto { Email = "john@example.com", FirstName = string.Empty, LastName = "Doe", Password = "Password1!", Role = UserRole.Technician };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_All_Fields_Valid()
    {
        var dto = new UserCreateDto
        {
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "SecurePass1!",
            Role = UserRole.Technician
        };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeTrue();
    }
}
