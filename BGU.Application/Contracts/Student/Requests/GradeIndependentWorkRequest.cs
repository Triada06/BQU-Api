using BGU.Core.Enums;
using FluentValidation;

namespace BGU.Application.Contracts.Student.Requests;

public sealed record GradeIndependentWorkRequest(
    string StudentId,
    string IndependentWorkId,
    bool? IsPassed);

public sealed class GradeIndependentWorkRequestValidator
    : AbstractValidator<GradeIndependentWorkRequest>
{
    public GradeIndependentWorkRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("StudentId is required.");

        RuleFor(x => x.IndependentWorkId)
            .NotEmpty().WithMessage("IndependentWorkId is required.");
    }
}