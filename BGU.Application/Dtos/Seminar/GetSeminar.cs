using BGU.Core.Enums;

namespace BGU.Application.Dtos.Seminar;

public sealed record GetSeminar(
    string Id,
    DateTime DueDate,
    Grade Grade,
    string StudentName,
    string StudentId,
    string TaughtSubjectId);