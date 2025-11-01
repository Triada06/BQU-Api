using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class StudentAcademicInfo : BaseEntity
{
    public int DecreeNumber { get; set; }
    
    public string FacultyId { get; set; }
    public Faculty Faculty { get; set; }

    public string SpecializationId { get; set; }
    public Specialization Specialization { get; set; }

    public string GroupId { get; set; }
    public Group Group { get; set; }

    public string AdmissionYearId  { get; set; }
    public AdmissionYear AdmissionYear { get; set; }

    public EducationLanguage EducationLanguage { get; set; }
    public FormOfEducation FormOfEducation { get; set; }

    public double Gpa { get; set; }
    public double AdmissionScore { get; set; }
}