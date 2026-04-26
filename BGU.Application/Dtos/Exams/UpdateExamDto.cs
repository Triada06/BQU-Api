using FluentValidation;

namespace BGU.Application.Dtos.Exams;

public sealed record UpdateExamDto(string StudentId, string TaughtSubjectId, DateTime Date, int Grade, bool IsAllowed);

public class UpdateExamDtoValidator : AbstractValidator<UpdateExamDto>
{
    public UpdateExamDtoValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("StudentId is required");
        RuleFor(x => x.TaughtSubjectId).NotEmpty().WithMessage("SubjectId is required");
        RuleFor(x => x.Date)
            .Must(x => x.Date >= DateTime.UtcNow)
            .WithMessage("Date must be in the future");
        RuleFor(x => x.Grade)
            .GreaterThan(0)
            .WithMessage("Grade must be greater than 0")
            .LessThanOrEqualTo(50)
            .WithMessage("Grade must be less than or equal 50");
    }
}