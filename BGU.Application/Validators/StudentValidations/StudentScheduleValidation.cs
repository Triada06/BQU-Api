using BGU.Application.Contracts.Student.Requests;
using FluentValidation;

namespace BGU.Application.Validators.StudentValidations;

public class StudentScheduleValidation : AbstractValidator<StudentScheduleRequest>
{
    public StudentScheduleValidation()
    {
        RuleFor(x => x.Schedule)
            .Must(r => r != null &&
                       (r.Equals("today", StringComparison.OrdinalIgnoreCase) ||
                        r.Equals("week", StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Schedule must be either 'today' or 'week'.")
            .NotEmpty().WithMessage("Schedule is required");
    }
}