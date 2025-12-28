using BGU.Core.Enums;
using FluentValidation;

namespace BGU.Application.Contracts.Student.Requests;

public record GradeSeminarRequest(string StudentId, string SeminarId, Grade Grade);

public sealed class GradeSeminarRequestValidator 
    : AbstractValidator<GradeSeminarRequest>
{
    public GradeSeminarRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("StudentId is required.");

        RuleFor(x => x.SeminarId)
            .NotEmpty().WithMessage("SeminarId is required.");

        RuleFor(x => x.Grade)
            .IsInEnum()
            .WithMessage("Grade value is invalid.");
    }
}