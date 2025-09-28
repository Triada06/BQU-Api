using FluentValidation;

namespace BGU.Application.Dtos.AppUser;

public class AppUserSignInDto
{
    public required string Email { get; set; }
    public required string PassWord { get; set; }
}

public class AppUserSignInDtoValidator : AbstractValidator<AppUserSignInDto>
{
    public AppUserSignInDtoValidator()
    {
        RuleFor(m=>m.Email)
            .NotNull().WithMessage("Email can't be null")
            .NotEmpty().WithMessage("Email format is invalid.");
        RuleFor(model => model.PassWord)
            .NotNull().WithMessage("Password can't be null")
            .NotEmpty().WithMessage("Password is required.");
    }
}