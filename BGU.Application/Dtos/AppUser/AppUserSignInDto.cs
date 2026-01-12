using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FluentValidation;

namespace BGU.Application.Dtos.AppUser;

public class AppUserSignInDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class AppUserSignInDtoValidator : AbstractValidator<AppUserSignInDto>
{
    public AppUserSignInDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Field is required.")
            .Must(BeValidFin)
            .WithMessage("Enter FIN code.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
    
    private bool BeValidFin(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        
        // FIN: 7 alphanumeric characters 
        var isFin = Regex.IsMatch(
            value,
            @"^[A-Z0-9]{7}$",
            RegexOptions.IgnoreCase
        );

        return isFin;
    }
}