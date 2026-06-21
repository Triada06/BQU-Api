namespace BGU.Application.Dtos.Transcript;

public record TranscriptQueryResult(
    string FullName,
    string StudentNumber,
    string Faculty,
    string Specialization,
    string Group,
    string EducationLanguage,
    string EducationLevel,
    int AdmissionYear,
    double Gpa,
    List<TranscriptRow> Rows);
