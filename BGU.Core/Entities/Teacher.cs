namespace BGU.Core.Entities;

public class Teacher : BaseEntity
{
    public string AppUserId { get; set; } = null!;
    public AppUser AppUser { get; set; } = null!;
    
    public TeacherAcademicInfo TeacherAcademicInfo { get; set; } = new();
    public ICollection<TaughtSubject> TaughtSubjects { get; set; } = [];
}