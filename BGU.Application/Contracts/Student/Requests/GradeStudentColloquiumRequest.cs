using BGU.Core.Enums;
using FluentValidation;

namespace BGU.Application.Contracts.Student.Requests;

public sealed record GradeStudentColloquiumRequest(string StudentId, string ColloquiumId, Grade Grade);


public sealed class GradeStudentColloquiumValidator 
    : AbstractValidator<GradeStudentColloquiumRequest>
{
    public GradeStudentColloquiumValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("StudentId is required.");

        RuleFor(x => x.ColloquiumId)
            .NotEmpty().WithMessage("ColloquiumId is required.");

        RuleFor(x => x.Grade)
            .IsInEnum()
            .WithMessage("Grade value is invalid.");
    }
}