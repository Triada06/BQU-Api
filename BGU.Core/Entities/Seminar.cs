using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class Seminar : BaseEntity
{
    public string? Topic { get; set; }
    
    public string StudentId { get; set; }
    public Student Student { get; set; }

    public string TaughtSubjectId { get; set; }
    public TaughtSubject TaughtSubject { get; set; }
    public DateTime GotAt { get; set; }

    public Grade Grade { get; set; }
}