namespace BGU.Application.Dtos.Exams;

public sealed record GetFinalDto(
    string Id,
    string GroupCode,
    string StudentId,
    string StudentName,
    string SubjectCode,
    bool IsConfirmed,
    string? FormattedDate,
    int Grade);