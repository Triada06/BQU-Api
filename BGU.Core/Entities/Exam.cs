namespace BGU.Core.Entities;

public class Exam : BaseEntity
{
    private int _Grade;

    public int Grade
    {
        get => _Grade;
        set
        {
            if (value is < -1 or > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(Grade));
            }

            _Grade = value;
        }
    }

    public DateTime? Date { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsAllowed { get; set; }

    // shows the exam that should be used. For instance, if student had a fail and a new exam created through 25% 
    // the actual status is going to be false for the failed exam 
    public bool IsActual { get; set; } = true;

    public string StudentId { get; set; }
    public Student Student { get; set; }
    public string TaughtSubjectId { get; set; }
    public TaughtSubject TaughtSubject { get; set; }
}