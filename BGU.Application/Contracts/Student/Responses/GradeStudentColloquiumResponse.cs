using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Student.Responses;

public sealed record GradeStudentColloquiumResponse(StatusCode StatusCode, bool IsSucceeded, string ResponserMessage);