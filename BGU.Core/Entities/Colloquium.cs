using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class Colloquium : BaseEntity
{
    public Grade Grade { get; set; }
    public DateTime Date { get; set; }
    public bool IsConfirmed { get; set; }
    
    public string StudentId { get; set; }
    public Student Student { get; set; }
    public string TaughtSubjectId { get; set; }
    public TaughtSubject TaughtSubject { get; set; }

}   