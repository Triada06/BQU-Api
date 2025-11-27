using BGU.Application.Contracts.Rooms.Requests;
using FluentValidation;

namespace BGU.Application.Validators.Room;

public class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomRequestValidator()
    {
        RuleFor(x => x.Capacity).NotEmpty().WithMessage("Capacity is required")
            .LessThan(30).WithMessage("Capacity must be greater than 30");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Room name is required")
            .MaximumLength(15).WithMessage("Room name cannot exceed 15 characters");
    }
}