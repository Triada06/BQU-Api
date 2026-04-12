namespace BGU.Core.Entities;

public class StudentSubjectEnrollment : BaseEntity
{
    public string StudentId { get; set; }
    public Student Student { get; set; }
    
    public string TaughtSubjectId { get; set; }
    public TaughtSubject TaughtSubject { get; set; }

    public int Attempt { get; set; }
}