using BGU.Core.Enums;

namespace BGU.Application.Dtos.Colloquiums;

public sealed record ColloquiumDto(
    string ColloquiumId,
    int OrderNumber,
    string TaughtSubjectName,
    string StudentFullName,
    Grade Grade,
    DateTime DateTime);