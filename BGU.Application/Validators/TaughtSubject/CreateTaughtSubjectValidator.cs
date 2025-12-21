using BGU.Application.Dtos.TaughtSubject.Requests;
using FluentValidation;

namespace BGU.Application.Validators.TaughtSubject;

public class CreateTaughtSubjectValidator : AbstractValidator<CreateTaughtSubjectRequest>
{
    public CreateTaughtSubjectValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Subject code is required.")
            .MaximumLength(20).WithMessage("Subject code must not exceed 20 characters.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Subject title is required.")
            .MaximumLength(100).WithMessage("Subject title must not exceed 100 characters.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department ID is required.");

        RuleFor(x => x.TeacherId)
            .NotEmpty().WithMessage("Teacher ID is required.");

        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("Group ID is required.");

        RuleFor(x => x.Hours)
            .GreaterThan(0).WithMessage("Hours must be greater than 0.")
            .LessThanOrEqualTo(300).WithMessage("Hours must not exceed 300.");
        
        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Credits must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Credits must not exceed 100.");
    }
}