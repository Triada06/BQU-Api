using FluentValidation;

namespace BGU.Application.Contracts.Seminars.Requests;

public sealed record CreateSeminarRequest(string StudentId, string TaughtSubjectId);

public sealed class CreateSeminarValidator
    : AbstractValidator<CreateSeminarRequest>
{
    public CreateSeminarValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("StudentId is required.");

        RuleFor(x => x.TaughtSubjectId)
            .NotEmpty().WithMessage("TaughtSubjectId is required.");
    }
}