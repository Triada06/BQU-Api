using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class Group : BaseEntity
{
    public required string Code { get; set; }
    public string AdmissionYearId { get; set; }
    public AdmissionYear AdmissionYear { get; set; }
    public EducationLanguage EducationLanguage { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public string SpecializationId { get; set; }
    public Specialization Specialization { get; set; }

    public ICollection<TaughtSubject> TaughtSubjects { get; set; } = [];
    public ICollection<StudentAcademicInfo> Students { get; set; } = [];    
}