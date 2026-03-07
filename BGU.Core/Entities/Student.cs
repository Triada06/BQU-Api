namespace BGU.Core.Entities;

public class Student : BaseEntity
{
    public string AppUserId { get; set; } = null!;
    public AppUser AppUser { get; set; } = null!;


    public string FacultyId { get; set; }
    public Faculty Faculty { get; set; }

    public string SpecializationId { get; set; }
    public Specialization Specialization { get; set; }

    public string GroupId { get; set; }
    public Group Group { get; set; }

    public string AdmissionYearId { get; set; }
    public AdmissionYear AdmissionYear { get; set; }

    public double Gpa { get; set; }
    public double AdmissionScore { get; set; }

    public ICollection<Attendance> Attendances { get; set; } = [];
    public ICollection<Colloquiums> Colloquiums { get; set; } = [];
    public ICollection<Seminar> SeminarGrades { get; set; } = [];
    public ICollection<IndependentWork> IndependentWorks { get; set; } = [];
    public ICollection<Exam> Finals { get; set; } = [];
}