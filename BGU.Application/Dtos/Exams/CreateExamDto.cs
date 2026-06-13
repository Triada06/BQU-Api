using FluentValidation;

namespace BGU.Application.Dtos.Exams;

public record CreateExamDto(string StudentId, string SubjectId, DateTime Date);

public class CreateExamDtoValidator : AbstractValidator<CreateExamDto>
{
    public CreateExamDtoValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("StudentId is required");
        RuleFor(x => x.SubjectId).NotEmpty().WithMessage("SubjectId is required");
    }
}