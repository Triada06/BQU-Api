namespace BGU.Application.Dtos.Transcript;

public record TranscriptRow(
    string SubjectName,
    string SubjectCode,
    int Credits,
    int Semester,
    double FinalGrade,
    string Letter);
