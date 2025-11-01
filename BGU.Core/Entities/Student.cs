namespace BGU.Core.Entities;

public class Student : BaseEntity
{
    public string AppUserId { get; set; } = null!;
    public AppUser AppUser { get; set; } = null!;
    
    public string StudentAcademicInfoid { get; set; } = null!;
    public StudentAcademicInfo StudentAcademicInfo { get; set; } 
}