using FluentValidation;

namespace BGU.Application.Contracts.IndependentWorks.Requests;

public sealed record GradeIndependentWorkRequest(string StudentId, string TaughtSubjectId, DateTime DueDate);

public sealed class CreateIndependentWorkRequestValidator 
    : AbstractValidator<GradeIndependentWorkRequest>
{
    public CreateIndependentWorkRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("StudentId is required.");

        RuleFor(x => x.TaughtSubjectId)
            .NotEmpty().WithMessage("TaughtSubjectId is required.");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("DueDate must be in the future.");
    }
}