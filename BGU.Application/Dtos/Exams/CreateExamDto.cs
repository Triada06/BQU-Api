using FluentValidation;

namespace BGU.Application.Dtos.Exams;

public record CreateExamDto(string StudentId, string SubjectId, DateTime Date);

public class CreateExamDtoValidator : AbstractValidator<CreateExamDto>
{
    public CreateExamDtoValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("StudentId is required");
        RuleFor(x => x.SubjectId).NotEmpty().WithMessage("SubjectId is required");
        RuleFor(x => x.Date)
            .Must(x => x.Date >= DateTime.UtcNow)
            .WithMessage("Date must be in the future");
    }
}