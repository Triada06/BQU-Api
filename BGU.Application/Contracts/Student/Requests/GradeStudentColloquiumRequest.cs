using BGU.Core.Enums;

namespace BGU.Application.Contracts.Student.Requests;

public sealed record GradeStudentColloquiumRequest(string StudentId, string ColloquiumId, Grade Grade);