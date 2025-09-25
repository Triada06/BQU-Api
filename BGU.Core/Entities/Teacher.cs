namespace BGU.Core.Entities;

public class Teacher : BaseEntity
{
    public string TeacherAcademicInfoId { get; set; }
    public TeacherAcademicInfo TeacherAcademicInfo { get; set; }
}