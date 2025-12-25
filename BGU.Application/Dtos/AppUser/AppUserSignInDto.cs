using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FluentValidation;

namespace BGU.Application.Dtos.AppUser;

public class AppUserSignInDto
{
    public required string EmailOrFin { get; set; }
    public required string PassWord { get; set; }
}

public class AppUserSignInDtoValidator : AbstractValidator<AppUserSignInDto>
{
    public AppUserSignInDtoValidator()
    {
        RuleFor(x => x.EmailOrFin)
            .NotEmpty().WithMessage("Field is required.")
            .Must(BeValidEmailOrFin)
            .WithMessage("Enter a valid email or FIN code.");

        RuleFor(x => x.PassWord)
            .NotEmpty().WithMessage("Password is required.");
    }
    
    private bool BeValidEmailOrFin(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        value = value.Trim();

        // Email check
        var emailValidator = new EmailAddressAttribute();
        var isEmail = emailValidator.IsValid(value);

        // FIN: 7 alphanumeric characters 
        var isFin = Regex.IsMatch(
            value,
            @"^[A-Z0-9]{7}$",
            RegexOptions.IgnoreCase
        );

        return isEmail || isFin;
    }
}