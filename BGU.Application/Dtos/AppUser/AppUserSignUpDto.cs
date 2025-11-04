using FluentValidation;

namespace BGU.Application.Dtos.AppUser;

public record AppUserSignUpDto(
    string UserName,
    string Email,
    string PassWord,
    string Name,
    string Surname,
    string MiddleName,
    string PinCode,
    char Gender);

public class AppUserCreateValidator : AbstractValidator<AppUserSignUpDto>
{
    public AppUserCreateValidator()
    {
        RuleFor(model => model.UserName)
            .NotEmpty().WithMessage("Username must not be empty.")
            .NotNull().WithMessage("Username must not be empty.")
            .MaximumLength(30).WithMessage("Username must not exceed 30 characters.");
            
        RuleFor(model => model.Email)
            .NotNull().WithMessage("Email address can't be null")
            .NotEmpty().WithMessage("Email address can't be empty.")
            .EmailAddress().WithMessage("Email address format is invalid.");
            
        RuleFor(model => model.PassWord)
            .NotNull().WithMessage("Password can't be null")
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.");
            
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
            
        RuleFor(model => model.MiddleName)
            .NotNull().WithMessage("Middle name is required.")
            .NotEmpty().WithMessage("Middle name must not be empty.")
            .MaximumLength(15).WithMessage("Middle name must not exceed 15 characters.")
            .Matches("^[a-zA-ZçÇğĞıİöÖşŞüÜəƏ]+$")
            .WithMessage("Middle name must contain only letters.");
            
        RuleFor(model => model.PinCode)
            .NotEmpty().WithMessage("FIN code is required.")
            .Length(7).WithMessage("FIN code must be exactly 7 characters.")
            .Matches("^[A-HJ-NP-Z0-9]{7}$")
            .WithMessage("FIN code must contain only capital letters and digits, and cannot include I, O, or Q.");
            
        RuleFor(model => model.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(g => g is 'M' or 'F' or 'm' or 'f')
            .WithMessage("Gender must be 'M' (Male) or 'F' (Female).");
    }
}