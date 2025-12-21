using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class TeacherAcademicInfo : BaseEntity
{
    public string DepartmentId { get; set; }
    public Department Department { get; set; }

    public TeachingPosition TeachingPosition { get; set; }
    public string TeacherId { get; set; }
    public Teacher Teacher { get; set; }
}