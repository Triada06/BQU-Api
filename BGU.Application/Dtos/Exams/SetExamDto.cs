using FluentValidation;

namespace BGU.Application.Dtos.Exams;

public sealed record SetExamDto(string Id, DateTime Date);

public class SetExamDtoValidator : AbstractValidator<SetExamDto>
{
    public SetExamDtoValidator()
    {
        RuleFor(x => x.Date)
            .Must(x => x.Date >= DateTime.UtcNow)
            .WithMessage("Date must be in the future");
    }
}