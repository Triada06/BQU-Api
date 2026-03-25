using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class Notification : BaseEntity
{
    public NotificationType Type { get; set; }
    public required string SubjectId { get; init; }
    public required string SubjectName { get; init; }
    public bool? MarkedAsPresence { get; set; }
    public Grade? Grade { get; set; }
    public DateTime? ClassDate { get; set; }

    public string Description => Type switch
    {
        NotificationType.Seminar =>
            $"Your {SubjectName} seminar was graded {Grade}",

        NotificationType.Attendance =>
            MarkedAsPresence is true
                ? $"You were marked present for {SubjectName} on {ClassDate:dd MMM yyyy}"
                : $"You were marked absent for {SubjectName} on {ClassDate:dd MMM yyyy}",

        NotificationType.IndependentWork =>
            $"Your {SubjectName} independent work was graded {Grade}",

        NotificationType.Colloquium =>
            $"Your {SubjectName} colloquium was graded {Grade}",

        _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, "Unhandled notification type")
    };
}