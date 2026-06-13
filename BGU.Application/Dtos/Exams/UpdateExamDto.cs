using FluentValidation;

namespace BGU.Application.Dtos.Exams;

public sealed record UpdateExamDto(string StudentId, string TaughtSubjectId, DateTime Date, int Grade, bool IsAllowed);

public class UpdateExamDtoValidator : AbstractValidator<UpdateExamDto>
{
    public UpdateExamDtoValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("StudentId is required");
        RuleFor(x => x.TaughtSubjectId).NotEmpty().WithMessage("SubjectId is required");
        RuleFor(x => x.Grade)
            .GreaterThanOrEqualTo(-1)
            .WithMessage("Grade must be greater than 0")
            .LessThanOrEqualTo(50)
            .WithMessage("Grade must be less than or equal 50");
    }
}