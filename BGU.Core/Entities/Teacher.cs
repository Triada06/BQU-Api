namespace BGU.Core.Entities;

public class Teacher : BaseEntity
{
    public string AppUserId { get; set; } = null!;
    public AppUser AppUser { get; set; } = null!;

    
    public string TeacherAcademicInfoId { get; set; }
    public TeacherAcademicInfo TeacherAcademicInfo { get; set; }
}