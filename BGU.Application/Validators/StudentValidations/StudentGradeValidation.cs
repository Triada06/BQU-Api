using BGU.Application.Contracts.Student.Requests;
using FluentValidation;

namespace BGU.Application.Validators.StudentValidations;

public class StudentGradeValidation : AbstractValidator<StudentGradesRequest>
{
    public StudentGradeValidation()
    {
        RuleFor(x => x.Grade)
            .NotEmpty().WithMessage("Grade is required")
            .Must(r => r != null &&
                       (r.Equals("current", StringComparison.OrdinalIgnoreCase) ||
                        r.Equals("session", StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Grade must be either 'current' or 'session'.");
    }
}