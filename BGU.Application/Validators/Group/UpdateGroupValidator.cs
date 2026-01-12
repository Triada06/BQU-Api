using BGU.Application.Contracts.Group.Requests;
using FluentValidation;

namespace BGU.Application.Validators.Group;

public class UpdateGroupValidator : AbstractValidator<UpdateGroupRequest>
{
    public UpdateGroupValidator()
    {
        RuleFor(x => x.GroupCode).NotEmpty().WithMessage("Group Code is required.")
            .MaximumLength(20).WithMessage("Group Code cannot exceed 20 characters.");
        RuleFor(x => x.SpecializationId).NotEmpty().WithMessage("SpecializationId is required.");
        RuleFor(x => x.Year).NotEmpty().WithMessage("Year is required.");
        
    }
}