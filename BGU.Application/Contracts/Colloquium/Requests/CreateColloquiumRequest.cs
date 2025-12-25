using BGU.Core.Enums;
using FluentValidation;

namespace BGU.Application.Contracts.Colloquium.Requests;

public sealed record CreateColloquiumRequest(string TaughtSubjectId, string StudentId, DateTime Date, Grade? Grade);

public class CreateColloquiumRequestValidator 
    : AbstractValidator<CreateColloquiumRequest>
{
    public CreateColloquiumRequestValidator()
    {
        RuleFor(x => x.TaughtSubjectId)
            .NotEmpty()
            .WithMessage("Taught subject is required.");

        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("Student is required.");

        RuleFor(x => x.Date)
            .NotEqual(default(DateTime))
            .WithMessage("Date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Date cannot be far in the future.");

        When(x => x.Grade is not null, () =>
        {
            RuleFor(x => x.Grade)
                .IsInEnum()
                .WithMessage("Grade is invalid.");
        });
    }
}