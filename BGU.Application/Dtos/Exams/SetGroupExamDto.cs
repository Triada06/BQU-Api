using FluentValidation;

namespace BGU.Application.Dtos.Exams;

public record SetGroupExamDto(string GroupId, string TaughtSubjectId, DateTime Date);

public class SetGroupExamDtoValidator : AbstractValidator<SetGroupExamDto>
{
    public SetGroupExamDtoValidator()
    {
        RuleFor(x=>x.GroupId).NotEmpty().WithMessage("GroupId is required");
        RuleFor(x=>x.TaughtSubjectId).NotEmpty().WithMessage("TaughtSubjectId is required");
    }
}