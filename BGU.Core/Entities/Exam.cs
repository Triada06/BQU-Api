using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class Exam : BaseEntity
{
    private int? _Grade;

    public int? Grade
    {
        get => _Grade;
        set
        {
            if (value is < 0 or > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(Grade));
            }
            _Grade = value;
        }
    }

    public DateTime? Date { get; set; }
    public bool IsConfirmed { get; set; }

    public string StudentId { get; set; }
    public Student Student { get; set; }
    public string TaughtSubjectId { get; set; }
    public TaughtSubject TaughtSubject { get; set; }
}