using BGU.Application.Contracts.Seminars.Requests;
using FluentValidation;

namespace BGU.Application.Contracts.Student.Requests;

public record GradeSeminarRequest(string SeminarId, GradeSeminarAndClassRequest SeminarData);

public sealed class GradeSeminarRequestValidator
    : AbstractValidator<GradeSeminarRequest>
{
    public GradeSeminarRequestValidator()
    {
        RuleFor(x => x.SeminarId)
            .NotEmpty().WithMessage("SeminarId is required.");
        
        RuleFor(x => x.SeminarData.StudentId)
            .NotEmpty().WithMessage("StudentId is required.");
        
        RuleFor(x => x.SeminarData.ClassId)
            .NotEmpty().WithMessage("ClassId is required.");

        RuleFor(x => x.SeminarData.Grade)
            .IsInEnum()
            .WithMessage("Grade value is invalid.");
    }
}