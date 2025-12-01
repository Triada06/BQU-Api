using BGU.Application.Contracts.Teacher.Requests;
using FluentValidation;

namespace BGU.Application.Validators.TeacherValidators;

public class UpdateTeacherRequestValidator : AbstractValidator<UpdateTeacherRequest>
{
    public UpdateTeacherRequestValidator()
    {
        RuleFor(model => model.Email)
            .NotNull().WithMessage("Email address can't be null")
            .NotEmpty().WithMessage("Email address can't be empty.")
            .EmailAddress().WithMessage("Email address format is invalid.");
        
        RuleFor(model => model.DepartmentId)
            .NotNull().WithMessage("Email address can't be null")
            .NotEmpty().WithMessage("Email address can't be empty.");

        RuleFor(model => model.Name)
            .NotNull().WithMessage("Name is required.")
            .NotEmpty().WithMessage("Name must not be empty.")
            .MaximumLength(15).WithMessage("Name must not exceed 15 characters.")
            .Matches("^[a-zA-ZçÇğĞıİöÖşŞüÜəƏ]+$")
            .WithMessage("Name must contain only letters.");

        RuleFor(model => model.Surname)
            .NotNull().WithMessage("Surname is required.")
            .NotEmpty().WithMessage("Surname must not be empty.")
            .MaximumLength(15).WithMessage("Surname must not exceed 15 characters.")
            .Matches("^[a-zA-ZçÇğĞıİöÖşŞüÜəƏ]+$")
            .WithMessage("Surname must contain only letters.");
    }
}