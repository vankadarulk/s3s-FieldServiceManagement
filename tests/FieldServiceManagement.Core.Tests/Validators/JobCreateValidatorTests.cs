using FieldServiceManagement.Core.DTOs;
using FieldServiceManagement.Core.Enums;
using FieldServiceManagement.Core.Validators;
using FluentAssertions;
using Xunit;

namespace FieldServiceManagement.Core.Tests.Validators;

public class JobCreateValidatorTests
{
    private readonly JobCreateValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        var dto = new JobCreateDto { Title = string.Empty, Priority = JobPriority.Medium };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Should_Fail_When_Title_Exceeds_200_Chars()
    {
        var dto = new JobCreateDto { Title = new string('a', 201), Priority = JobPriority.Medium };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Should_Fail_When_Description_Exceeds_2000_Chars()
    {
        var dto = new JobCreateDto
        {
            Title = "Valid Title",
            Description = new string('a', 2001),
            Priority = JobPriority.Medium
        };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Should_Fail_When_ScheduledDate_Is_In_The_Past()
    {
        var dto = new JobCreateDto
        {
            Title = "Valid Title",
            Priority = JobPriority.Medium,
            ScheduledDate = DateTime.Today.AddDays(-1)
        };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ScheduledDate");
    }

    [Fact]
    public void Should_Pass_When_ScheduledDate_Is_Today()
    {
        var dto = new JobCreateDto
        {
            Title = "Valid Title",
            Priority = JobPriority.Medium,
            ScheduledDate = DateTime.Today
        };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_All_Fields_Valid()
    {
        var dto = new JobCreateDto
        {
            Title = "Fix HVAC Unit",
            Description = "Replace filters and check coolant levels.",
            Priority = JobPriority.High,
            ScheduledDate = DateTime.Today.AddDays(3)
        };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeTrue();
    }
}
