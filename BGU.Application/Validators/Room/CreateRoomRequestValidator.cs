using BGU.Application.Contracts.Rooms.Requests;
using FluentValidation;

namespace BGU.Application.Validators.Room;

public class CreateRoomRequestValidator : AbstractValidator<CreateRoomRequest>
{
    public CreateRoomRequestValidator()
    {
        RuleFor(x => x.Capacity).NotEmpty().WithMessage("Capacity is required")
            .GreaterThan(0).WithMessage("Capacity must be greater than zero")
            .LessThan(50).WithMessage("Capacity must be less than 50");
        RuleFor(x => x.RoomName).NotEmpty().WithMessage("Room name is required")
            .MaximumLength(15).WithMessage("Room name cannot exceed 15 characters");
    }
}