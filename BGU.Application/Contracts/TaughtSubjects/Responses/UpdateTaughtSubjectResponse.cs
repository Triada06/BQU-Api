using BGU.Application.Dtos.TaughtSubject;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.TaughtSubjects.Responses;

public record UpdateTaughtSubjectResponse(
    string? Id,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);