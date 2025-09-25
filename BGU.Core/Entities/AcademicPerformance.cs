using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class AcademicPerformance : BaseEntity
{
    public string StudentId { get; set; }
    public Student Student { get; set; }

    public string ClassId { get; set; }
    public Class Class { get; set; }
    public Grade Grade { get; set; }
    public Attendance Attendance { get; set; }
    public bool IsConfirmed { get; set; }
}