namespace BGU.Core.Entities;

public class IndependentWork : BaseEntity
{
    public bool IsAccepted { get; set; }
    public bool IsConfirmed { get; set; }
    public DateTime Date { get; set; }
    
    public string StudentId { get; set; }
    public Student Student { get; set; }

    public string TaughtSubjectId { get; set; }
    public TaughtSubject TaughtSubject { get; set; }

}