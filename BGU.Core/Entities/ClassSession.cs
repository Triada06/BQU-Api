namespace BGU.Core.Entities;

public class ClassSession : BaseEntity
{
    public string TaughtSubjectId { get; set; }
    public TaughtSubject TaughtSubject { get; set; }
    public DateTimeOffset Date { get; set; }
    public ICollection<Attendance> Attendances { get; set; } = [];
}