using FieldServiceManagement.Core.DTOs;
using FluentValidation;

namespace FieldServiceManagement.Core.Validators;

public class JobCreateValidator : AbstractValidator<JobCreateDto>
{
    public JobCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => x.Description != null);

        RuleFor(x => x.ScheduledDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Scheduled date must be today or in the future.")
            .When(x => x.ScheduledDate.HasValue);

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority is required and must be valid.");
    }
}
