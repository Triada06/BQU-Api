using BGU.Application.Contracts.Group.Requests;
using FluentValidation;

namespace BGU.Application.Validators.Group;

public class CreateGroupValidator : AbstractValidator<CreateGroupRequest>
{
    public CreateGroupValidator()
    {
        RuleFor(x => x.GroupCode).NotNull().WithMessage("GroupCode is required");
        RuleFor(x => x.DepartmentId).NotNull().WithMessage("DepartmentId is required");
        RuleFor(x => x.Year).NotNull().WithMessage("Year is required")
            .LessThanOrEqualTo(4).WithMessage("Year must be greater than or equal to 4")
            .GreaterThan(0).WithMessage("Year must be greater than 0");
    }
}