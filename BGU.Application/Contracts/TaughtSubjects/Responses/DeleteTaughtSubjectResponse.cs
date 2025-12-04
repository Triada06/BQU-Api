using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.TaughtSubjects.Responses;

public sealed record DeleteTaughtSubjectResponse(
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);