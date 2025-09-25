namespace BGU.Core.Entities;

public class Student : BaseEntity
{
    public string AcademicInfoId { get; set; } = null!;
    public StudentAcademicInfo StudentAcademicInfo { get; set; }
}