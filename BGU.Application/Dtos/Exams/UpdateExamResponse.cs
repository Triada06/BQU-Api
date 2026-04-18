namespace BGU.Application.Dtos.Exams;

public sealed record UpdateExamResponse(string Id, string StudentId, string TaughtSubjectId, DateTime? Date, int? Grade);