namespace BGU.Application.Dtos.Exams;

public sealed record ExamsToGrade(IEnumerable<ExamToGrade> Exams);

public sealed record ExamToGrade(
    string Id,
    string StudentId,
    string StudentFullName,
    string SubjectId,
    string SubjectName,
    string SubjectCode,
    string GroupId,
    string GroupCode,
    int Grade,
    string FormattedDate);