using BGU.Core.Enums;
using FluentValidation;

namespace BGU.Application.Contracts.Notification.Requests;

public sealed record SendNotificationRequest(string From, string To, NotificationType NotificationType, string Message);


public class SendNotificationValidator : AbstractValidator<SendNotificationRequest>
{
    public SendNotificationValidator()
    {
        RuleFor(x=>x.To).NotEmpty().WithMessage("To is required");
        RuleFor(x=>x.From).NotEmpty().WithMessage("From is required");
        RuleFor(x=>x.Message)
            .NotEmpty().WithMessage("Message is required")
            .MaximumLength(200).WithMessage("Message cannot exceed 200 characters");;
    }
}