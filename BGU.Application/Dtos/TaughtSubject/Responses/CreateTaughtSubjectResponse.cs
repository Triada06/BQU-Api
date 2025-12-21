using BGU.Infrastructure.Constants;

namespace BGU.Application.Dtos.TaughtSubject.Responses;

public record CreateTaughtSubjectResponse(
    string? TaughtSubjectId,
    bool IsSucceeded,
    StatusCode StatusCode,
    string ErrorMessage);