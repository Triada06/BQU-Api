using BGU.Application.Contracts.Teacher.Requests;
using FluentValidation;

namespace BGU.Application.Validators.TeacherValidators;

public class TeacherScheduleValidator : AbstractValidator<TeacherScheduleRequest>
{
    public TeacherScheduleValidator()
    {
        RuleFor(x => x.Schedule)
            .Must(r => r != null &&
                       (r.Equals("today", StringComparison.OrdinalIgnoreCase) ||
                        r.Equals("week", StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Schedule must be either 'today' or 'week'.")
            .NotEmpty().WithMessage("Schedule is required");
    }
}