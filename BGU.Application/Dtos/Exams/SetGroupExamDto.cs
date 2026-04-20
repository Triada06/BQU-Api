using FluentValidation;

namespace BGU.Application.Dtos.Exams;

public record SetGroupExamDto(string GroupId, DateTime Date);

public class SetGroupExamDtoValidator : AbstractValidator<SetGroupExamDto>
{
    public SetGroupExamDtoValidator()
    {
        RuleFor(x=>x.GroupId).NotEmpty().WithMessage("GroupId is required");
        
        RuleFor(x => x.Date)
            .Must(x => x.Date >= DateTime.UtcNow)
            .WithMessage("Date must be in the future");
    }
}