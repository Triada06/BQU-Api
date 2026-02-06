using FluentValidation;

namespace BGU.Application.Contracts.IndependentWorks.Requests;

public sealed record GradeIndependentWorkRequest(string StudentId, string TaughtSubjectId, int Number);

public sealed class CreateIndependentWorkRequestValidator
    : AbstractValidator<GradeIndependentWorkRequest>
{
    public CreateIndependentWorkRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("StudentId is required.");

        RuleFor(x => x.TaughtSubjectId)
            .NotEmpty().WithMessage("TaughtSubjectId is required.");

        RuleFor(request => request.Number)
            .GreaterThan(0).WithMessage("Number must be greater than 0")
            .LessThan(11).WithMessage("Number must be less than 11");
    }
}