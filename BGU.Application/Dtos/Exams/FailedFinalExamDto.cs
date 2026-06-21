namespace BGU.Application.Dtos.Exams;

public sealed record FailedFinalExamDto(
    string Id,
    string StudentId,
    string StudentName,
    string GroupId,
    string GroupCode,
    string TaughtSubjectId,
    string SubjectId,
    string SubjectCode,
    string SubjectName,
    double GradeBeforeExam,
    double ExamGrade,
    double FinalGrade,
    DateTime? Date,
    int? Grade);
