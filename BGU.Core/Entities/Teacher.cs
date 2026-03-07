using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class Teacher : BaseEntity
{
    public string AppUserId { get; set; } = null!;
    public AppUser AppUser { get; set; } = null!;

    public string DepartmentId { get; set; }
    public Department Department { get; set; }

    public TeachingPosition TeachingPosition { get; set; }

    public ICollection<TaughtSubject> TaughtSubjects { get; set; } = [];
}