namespace BGU.Core.Entities;

public class Student : BaseEntity
{
    public string AppUserId { get; set; } = null!;
    public AppUser AppUser { get; set; } = null!;

    public StudentAcademicInfo StudentAcademicInfo { get; set; } = new();
    public ICollection<Attendance> Attendances { get; set; } = [];
    public ICollection<Colloquiums> Colloquiums { get; set; } = [];
    public ICollection<Seminar> SeminarGrades { get; set; } = [];
    public ICollection<IndependentWork> IndependentWorks { get; set; } = [];
} 