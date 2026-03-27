using FluentValidation;

namespace BGU.Application.Dtos.AppUser;

public sealed record AddEmailRequest(string Email);

public class AddEmailReqeustValidator : AbstractValidator<AddEmailRequest>
{
    public AddEmailReqeustValidator()
    {
        RuleFor(x=>x.Email).EmailAddress().WithMessage("Invalid email");
    }
}