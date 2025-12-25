using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class Class : BaseEntity
{
    public ClassType ClassType { get; set; }

    public string TaughtSubjectId { get; set; }
    public TaughtSubject TaughtSubject { get; set; }

    public string ClassTimeId { get; set; }
    public ClassTime ClassTime { get; set; }

    // public ICollection<ClassTime> ClassTimes { get; set; } = [];
}